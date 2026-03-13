import { describe, expect, it, vi } from "vitest";
import { runModules } from "./module";

describe("runModules", () => {
    it("runs only modules whose shouldRun returns true", () => {
        const firstRun = vi.fn();
        const secondRun = vi.fn();

        const firstModule = {
            shouldRun: () => true,
            run: firstRun
        };

        const secondModule = {
            shouldRun: () => false,
            run: secondRun
        };

        runModules([firstModule, secondModule]);

        expect(firstRun).toHaveBeenCalledTimes(1);
        expect(secondRun).not.toHaveBeenCalled();
    });
});
