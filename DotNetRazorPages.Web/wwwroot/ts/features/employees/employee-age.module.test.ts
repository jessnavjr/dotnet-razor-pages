import { beforeEach, describe, expect, it, vi } from "vitest";
import { createEmployeeAgeModule } from "./employee-age.module";

describe("createEmployeeAgeModule", () => {
    beforeEach(() => {
        document.body.innerHTML = "";
        vi.useFakeTimers();
        vi.setSystemTime(new Date("2026-03-13T12:00:00Z"));
    });

    it("shouldRun is true when employees table marker exists", () => {
        document.body.innerHTML = "<table data-module='employees-age'></table>";

        const module = createEmployeeAgeModule();

        expect(module.shouldRun()).toBe(true);
    });

    it("shouldRun is false when employees table marker does not exist", () => {
        const module = createEmployeeAgeModule();

        expect(module.shouldRun()).toBe(false);
    });

    it("writes calculated age when a valid date exists", () => {
        document.body.innerHTML =
            "<table data-module='employees-age'><tbody><tr>" +
            "<td data-role='employee-age' data-employee-date='2020-03-15'>--</td>" +
            "</tr></tbody></table>";

        const module = createEmployeeAgeModule();
        module.run();

        const cell = document.querySelector("[data-role='employee-age']");
        expect(cell?.textContent).toBe("5");
    });

    it("writes N/A when date is missing or invalid", () => {
        document.body.innerHTML =
            "<table data-module='employees-age'><tbody><tr>" +
            "<td data-role='employee-age'>--</td>" +
            "<td data-role='employee-age' data-employee-date='invalid'>--</td>" +
            "</tr></tbody></table>";

        const module = createEmployeeAgeModule();
        module.run();

        const cells = Array.from(document.querySelectorAll("[data-role='employee-age']"));
        expect(cells[0].textContent).toBe("N/A");
        expect(cells[1].textContent).toBe("N/A");
    });
});
