import { chromium } from 'playwright';

const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();
await page.setViewportSize({ width: 1440, height: 900 });

// --- 1. Initial load ---
await page.goto('http://localhost:5173', { waitUntil: 'networkidle' });
await page.screenshot({ path: './ss-01-initial.png', fullPage: true });

const critCount = await page.locator('.summary-chip--critical .summary-chip__count').textContent();
const warnCount = await page.locator('.summary-chip--warning .summary-chip__count').textContent();
const infoCount = await page.locator('.summary-chip--info .summary-chip__count').textContent();
const totalText = await page.locator('.total-count').textContent();
const cardCount = await page.locator('.alert-card').count();

console.log(`Total text: ${totalText}`);
console.log(`Cards rendered: ${cardCount}`);
console.log(`Critical: ${critCount}, Warning: ${warnCount}, Info: ${infoCount}`);

const critBorder = await page.locator('.alert-card--critical').first().evaluate(el =>
  getComputedStyle(el).getPropertyValue('border-left-color')
);
const warnBorder = await page.locator('.alert-card--warning').first().evaluate(el =>
  getComputedStyle(el).getPropertyValue('border-left-color')
);
const infoBorder = await page.locator('.alert-card--info').first().evaluate(el =>
  getComputedStyle(el).getPropertyValue('border-left-color')
);
console.log(`Critical border: ${critBorder}`);
console.log(`Warning border:  ${warnBorder}`);
console.log(`Info border:     ${infoBorder}`);

// --- 2. Expand items ---
const warnCard = page.locator('.alert-card--warning').first();
await warnCard.locator('.items-toggle').first().click();
await page.screenshot({ path: './ss-02-items-expanded.png', fullPage: true });
const itemRows = await warnCard.locator('.item-row').count();
console.log(`Item rows visible after expand: ${itemRows}`);

// --- 3. Filter by Critical ---
await page.locator('.summary-chip--critical').click();
await page.screenshot({ path: './ss-03-filter-critical.png' });
const critCardsAfterFilter = await page.locator('.alert-card').count();
const nonCritVisible = await page.locator('.alert-card--warning, .alert-card--info').count();
console.log(`Cards after critical filter: ${critCardsAfterFilter} (non-critical: ${nonCritVisible})`);

// --- 4. Filter by category "estoque" ---
await page.locator('.summary-chip--critical').click(); // toggle off
const estoquePill = page.locator('.pill').filter({ hasText: 'estoque' });
await estoquePill.click();
await page.screenshot({ path: './ss-04-filter-estoque.png' });
const estoquePillActive = await estoquePill.getAttribute('class');
const cardsAfterCatFilter = await page.locator('.alert-card').count();
console.log(`Cards after 'estoque' filter: ${cardsAfterCatFilter}`);
console.log(`Estoque pill active: ${estoquePillActive?.includes('pill--active')}`);

// --- 5. Dismiss a card ---
await page.locator('.pill').filter({ hasText: 'Todas' }).click();
const cardsBefore = await page.locator('.alert-card').count();
await page.locator('.dismiss-btn').first().click();
await page.waitForTimeout(400);
const cardsAfterDismiss = await page.locator('.alert-card').count();
console.log(`Cards before dismiss: ${cardsBefore}, after: ${cardsAfterDismiss}`);
await page.screenshot({ path: './ss-05-after-dismiss.png' });

// --- 6. Probe: empty state (cotacao + critical filter combo) ---
await page.locator('.summary-chip--critical').click();
const cotacaoPill = page.locator('.pill').filter({ hasText: 'cotacao' });
if (await cotacaoPill.count() > 0) {
  await cotacaoPill.click();
  await page.screenshot({ path: './ss-06-empty-state.png' });
  const emptyStateVisible = await page.locator('.empty-state').isVisible().catch(() => false);
  console.log(`Empty state visible: ${emptyStateVisible}`);
}

await browser.close();
console.log('DONE');
