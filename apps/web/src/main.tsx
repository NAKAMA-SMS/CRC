import { StrictMode, useEffect, useState } from "react";
import { createRoot } from "react-dom/client";

function Foundation() {
  const [status, setStatus] = useState("Checking service readiness…");
  useEffect(() => {
    const controller = new AbortController();
    async function check() {
      try {
        const response = await fetch("/health/ready", {
          signal: controller.signal,
          cache: "no-store",
        });
        const body: unknown = await response.json();
        const ready =
          response.ok &&
          typeof body === "object" &&
          body !== null &&
          "data" in body &&
          typeof body.data === "object" &&
          body.data !== null &&
          "status" in body.data &&
          body.data.status === "ready";
        setStatus(ready ? "Service ready" : "Service unavailable");
      } catch {
        if (!controller.signal.aborted) setStatus("Service unavailable");
      }
    }
    void check();
    return () => controller.abort();
  }, []);
  return (
    <main>
      <h1>CRC Foundation</h1>
      <p>CRC powered by NAKAMA</p>
      <p role="status">{status}</p>
    </main>
  );
}

const root = document.getElementById("root");
if (!root) throw new Error("Missing application root.");
createRoot(root).render(
  <StrictMode>
    <Foundation />
  </StrictMode>,
);
