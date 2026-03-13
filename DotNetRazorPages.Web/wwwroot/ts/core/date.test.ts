import { describe, expect, it } from "vitest";
import { calculateAgeInYears, parseIsoDate } from "./date";

describe("calculateAgeInYears", () => {
    it("returns the full age when birthday has passed", () => {
        const birthDate = new Date("2000-03-10T00:00:00Z");
        const atDate = new Date("2026-03-13T12:00:00Z");

        expect(calculateAgeInYears(birthDate, atDate)).toBe(26);
    });

    it("subtracts one when birthday has not occurred yet", () => {
        const birthDate = new Date("2000-12-01T00:00:00Z");
        const atDate = new Date("2026-03-13T12:00:00Z");

        expect(calculateAgeInYears(birthDate, atDate)).toBe(25);
    });
});

describe("parseIsoDate", () => {
    it("returns a Date for a valid ISO date", () => {
        const result = parseIsoDate("2020-03-15");

        expect(result).not.toBeNull();
        expect(result?.toISOString().startsWith("2020-03-15")).toBe(true);
    });

    it("returns null for invalid date text", () => {
        expect(parseIsoDate("not-a-date")).toBeNull();
    });
});
