import { defineConfig } from "@playwright/test";

if (!process.env.CRC_BROWSER_BASE_URL) {
  throw new Error(
    "CRC_BROWSER_BASE_URL must identify the isolated running fixture.",
  );
}
const url = new URL(process.env.CRC_BROWSER_BASE_URL);
if (url.hostname !== "127.0.0.1" && url.hostname !== "localhost") {
  throw new Error("Browser tests require a disposable loopback fixture.");
}
export default defineConfig({
  testDir: ".",
  testMatch: "*.spec.ts",
  fullyParallel: false,
  use: { baseURL: url.origin, browserName: "chromium", channel: "chromium" },
  reporter: [
    ["list"],
    ["junit", { outputFile: "../../artifacts/evidence/browser.xml" }],
  ],
});
