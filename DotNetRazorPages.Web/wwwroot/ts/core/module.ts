export interface AppModule {
    shouldRun: () => boolean;
    run: () => void;
}

export function runModules(modules: readonly AppModule[]): void {
    for (const module of modules) {
        if (module.shouldRun()) {
            module.run();
        }
    }
}
