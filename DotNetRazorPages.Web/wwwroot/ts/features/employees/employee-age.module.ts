import { calculateAgeInYears, parseIsoDate } from "../../core/date";
import type { AppModule } from "../../core/module";

const EMPLOYEE_TABLE_SELECTOR = "[data-module='employees-age']";
const EMPLOYEE_AGE_CELL_SELECTOR = "[data-role='employee-age']";

export function createEmployeeAgeModule(): AppModule {
    return {
        shouldRun: () => document.querySelector(EMPLOYEE_TABLE_SELECTOR) !== null,
        run: () => {
            const ageCells = document.querySelectorAll<HTMLTableCellElement>(EMPLOYEE_AGE_CELL_SELECTOR);

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
