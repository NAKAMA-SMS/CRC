import { test, expect } from "@playwright/test";

test("built shell uses only local assets and reports actual readiness", async ({
  page,
  baseURL,
}) => {
  const external: string[] = [];
  await page.route("**/*", async (route) => {
    if (new URL(route.request().url()).origin !== baseURL) {
      external.push(route.request().url());
      await route.abort();
    } else await route.continue();
  });
  const response = await page.goto("/");
  await expect(
    page.getByRole("heading", { name: "CRC Foundation" }),
  ).toBeVisible();
  await expect(page.getByRole("status")).toHaveText("Service ready");
  expect(external).toEqual([]);
  expect(response?.headers()["content-security-policy"]).toContain(
    "script-src 'self'",
  );
  expect(response?.headers()["x-content-type-options"]).toBe("nosniff");
  expect(response?.headers()["referrer-policy"]).toBe("no-referrer");
  const missing = await page.request.get("/api/v1/missing");
  expect(missing.status()).toBe(404);
  expect((await missing.json()).error.code).toBe("RESOURCE_NOT_FOUND");
  for (const path of [
    "/test-only/throw",
    "/test-only/protected",
    "/openapi/v1.json",
  ]) {
    expect((await page.request.get(path)).status()).toBe(404);
  }
});
