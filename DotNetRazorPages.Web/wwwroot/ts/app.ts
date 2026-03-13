import { runModules } from "./core/module";
import { createEmployeeAgeModule } from "./features/employees/employee-age.module";

const modules = [
    createEmployeeAgeModule()
];

runModules(modules);
