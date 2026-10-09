{
    "defaultIgnores": true,
    "ignorePatterns": ["dist/*", "*.js", "*.js.map"],
    "overrides": [
        {
            "files": ["*.ts"],
            "languageOptions": {
                "sourceType": "module",
                "ecmaVersion": "latest",
                "globals": {
                    "require": "readonly",
                    "process": "readonly"
                }
            },
            "plugins": ["@typescript-eslint"],
            "rules": {
                "no-unused-vars": "off",
                "@typescript-eslint/no-unused-vars": "warn"
            }
        }
    ]
}
