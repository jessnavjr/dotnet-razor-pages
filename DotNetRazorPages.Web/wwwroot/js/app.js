const EMPLOYEE_TABLE_SELECTOR = "[data-module='employees-age']";
const EMPLOYEE_AGE_CELL_SELECTOR = "[data-role='employee-age']";

function runModules(modules) {
    for (const module of modules) {
        if (module.shouldRun()) {
            module.run();
        }
    }
}

function calculateAgeInYears(fromDate, atDate = new Date()) {
    let years = atDate.getFullYear() - fromDate.getFullYear();

    const monthDelta = atDate.getMonth() - fromDate.getMonth();
    const dayDelta = atDate.getDate() - fromDate.getDate();

    if (monthDelta < 0 || (monthDelta === 0 && dayDelta < 0)) {
        years -= 1;
    }

    return years;
}

function parseIsoDate(value) {
    const parsed = new Date(value);

    if (Number.isNaN(parsed.getTime())) {
        return null;
    }

    return parsed;
}

function createEmployeeAgeModule() {
    return {
        shouldRun: () => document.querySelector(EMPLOYEE_TABLE_SELECTOR) !== null,
        run: () => {
            const ageCells = document.querySelectorAll(EMPLOYEE_AGE_CELL_SELECTOR);

            for (const ageCell of ageCells) {
                const rawDate = ageCell.dataset.employeeDate;

                if (!rawDate) {
                    ageCell.textContent = "N/A";
                    continue;
                }

                const parsedDate = parseIsoDate(rawDate);

                if (!parsedDate) {
                    ageCell.textContent = "N/A";
                    continue;
                }

                ageCell.textContent = String(calculateAgeInYears(parsedDate));
            }
        }
    };
}

runModules([
    createEmployeeAgeModule()
]);
