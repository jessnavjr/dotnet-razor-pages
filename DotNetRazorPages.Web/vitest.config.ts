import { defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        environment: "jsdom",
        include: ["wwwroot/ts/**/*.test.ts"]
    }
});
