const js = require("@eslint/js");
const ts = require("typescript-eslint");
const hooks = require("eslint-plugin-react-hooks");
module.exports = ts.config(
  { ignores: ["**/dist/**", "**/test-results/**", "**/playwright-report/**"] },
  js.configs.recommended,
  ...ts.configs.recommended,
  {
    files: ["**/*.tsx"],
    plugins: { "react-hooks": hooks },
    rules: hooks.configs.recommended.rules,
  },
);
