export function calculateAgeInYears(fromDate: Date, atDate: Date = new Date()): number {
    let years = atDate.getFullYear() - fromDate.getFullYear();

    const monthDelta = atDate.getMonth() - fromDate.getMonth();
    const dayDelta = atDate.getDate() - fromDate.getDate();

    if (monthDelta < 0 || (monthDelta === 0 && dayDelta < 0)) {
        years -= 1;
    }

    return years;
}

export function parseIsoDate(value: string): Date | null {
    const parsed = new Date(value);

    if (Number.isNaN(parsed.getTime())) {
        return null;
    }

    return parsed;
}
