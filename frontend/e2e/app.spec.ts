import { test, expect } from '@playwright/test';

test.describe('App', () => {
  test('should load and show navigation', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('nav')).toBeVisible();
    await expect(page.getByRole('link', { name: 'Início' })).toBeVisible();
    await expect(page.getByRole('link', { name: 'Configurações' })).toBeVisible();
  });

  test('should show home content', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByRole('heading', { name: 'Diário de Bordo' })).toBeVisible();
  });
});
