export function formatValueWithK(value: number): string {
  if (value >= 1000) {
    const newValue = (value / 1000).toFixed(1);
    return newValue.endsWith(".0")
      ? `${newValue.slice(0, -2)}<strong class="text-2xl">K+</strong>`
      : `${newValue}<strong class="text-2xl">K+</strong>`;
  } else {
    return value.toString();
  }
}
