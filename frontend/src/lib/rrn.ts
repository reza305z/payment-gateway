export function generateRrn(): string {
  return Array.from({ length: 12 }, (_, i) =>
    i === 0
      ? 1 + Math.floor(Math.random() * 9)
      : Math.floor(Math.random() * 10)
  ).join('');
}