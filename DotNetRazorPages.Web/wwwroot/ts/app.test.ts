describe("app bootstrap", () => {
    beforeEach(() => {
        jest.resetModules();
    });

    it("creates the employee age module and runs registered modules", () => {
        const employeeAgeModule = {
            shouldRun: jest.fn(() => true),
            run: jest.fn()
        };

        const runModules = jest.fn();
        const createEmployeeAgeModule = jest.fn(() => employeeAgeModule);

        jest.doMock("./core/module", () => ({
            runModules
        }));

        jest.doMock("./features/employees/employee-age.module", () => ({
            createEmployeeAgeModule
        }));

        jest.isolateModules(() => {
            require("./app");
        });

        expect(createEmployeeAgeModule).toHaveBeenCalledTimes(1);
        expect(runModules).toHaveBeenCalledTimes(1);
        expect(runModules).toHaveBeenCalledWith([employeeAgeModule]);
    });
});