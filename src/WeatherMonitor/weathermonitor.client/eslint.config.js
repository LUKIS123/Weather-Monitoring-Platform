// @ts-check
import pkg from "@eslint/js";
const { configs } = pkg;
import { config, configs as _configs } from "typescript-eslint";
import { configs as __configs, processInlineTemplates } from "angular-eslint";

export default config(
  {
    files: ["**/*.ts"],
    extends: [
      configs.recommended,
      // @ts-ignore
      ..._configs.recommended,
      // @ts-ignore
      ..._configs.stylistic,
      // @ts-ignore
      ...__configs.tsRecommended,
    ],
    processor: processInlineTemplates,
    rules: {
      "@angular-eslint/directive-selector": [
        "error",
        {
          type: "attribute",
          prefix: "app",
          style: "camelCase",
        },
      ],
      "@angular-eslint/component-selector": [
        "error",
        {
          type: "element",
          prefix: "app",
          style: "kebab-case",
        },
      ],
    },
  },
  {
    files: ["**/*.html"],
    extends: [
      ...__configs.templateRecommended,
      ...__configs.templateAccessibility,
    ],
    rules: {},
  }
);
