import { runModules } from "./module";

describe("runModules", () => {
    it("runs only modules whose shouldRun returns true", () => {
        const firstRun = jest.fn();
        const secondRun = jest.fn();

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
