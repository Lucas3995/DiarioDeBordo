import { test, expect } from '@playwright/test';

const backendAvailable = !!process.env.WITH_BACKEND;

test.describe('Módulo Obras — Listagem paginada', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/obras');
  });

  test('deve exibir o título da página de obras', async ({ page }) => {
    await expect(page.locator('h2')).toContainText('Acompanhamento de Obras');
  });

  test('deve exibir o link "Obras" no menu de navegação', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByRole('link', { name: 'Obras' })).toBeVisible();
  });

  test('deve navegar para /obras ao clicar no link "Obras"', async ({ page }) => {
    await page.goto('/');
    await page.getByRole('link', { name: 'Obras' }).click();
    await expect(page).toHaveURL(/\/obras/);
    await expect(page.locator('h2')).toContainText('Acompanhamento de Obras');
  });

  test.describe('dados da API (requer backend)', () => {
    test.skip(!backendAvailable, 'Requer backend rodando. Defina WITH_BACKEND=1 para executar.');

    test('deve exibir a tabela de obras com pelo menos um registro', async ({ page }) => {
      const tabela = page.locator('[data-testid="obras-tabela"]');
      await expect(tabela).toBeVisible();

      const linhas = page.locator('[data-testid="obra-nome"]');
      await expect(linhas.first()).toBeVisible();
      await expect(linhas).toHaveCount(10);
    });

    test('deve exibir o nome "One Piece" na primeira linha', async ({ page }) => {
      const primeiroNome = page.locator('[data-testid="obra-nome"]').first();
      await expect(primeiroNome).toHaveText('One Piece');
    });

    test('deve exibir tipos das obras', async ({ page }) => {
      const tipos = page.locator('[data-testid="obra-tipo"]');
      await expect(tipos.first()).toBeVisible();
    });

    test('deve exibir posição formatada com unidade do tipo (ex.: "capítulo 1110")', async ({
      page,
    }) => {
      const primeiraPosicao = page.locator('[data-testid="obra-posicao"]').first();
      await expect(primeiraPosicao).toHaveText('capítulo 1110');
    });

    test('deve exibir data relativa na coluna de última atualização', async ({ page }) => {
      const primeiraData = page.locator('[data-testid="obra-ultima-atualizacao"]').first();
      const texto = await primeiraData.textContent();
      expect(texto?.trim()).toBeTruthy();
      expect(texto?.trim()).toMatch(/há \d+ dias?|há \d+ (meses|anos?)|hoje/);
    });

    test('deve exibir a data em formato dd/MM/yyyy no tooltip ao hover', async ({ page }) => {
      const primeiraData = page.locator('[data-testid="obra-ultima-atualizacao"]').first();
      const title = await primeiraData.getAttribute('title');
      expect(title).toMatch(/^\d{2}\/\d{2}\/\d{4}$/);
    });

    test('deve exibir coluna Previsão com dias para obras com dias_ate_proxima', async ({
      page,
    }) => {
      const primeiraPrevisao = page.locator('[data-testid="obra-previsao"]').first();
      await expect(primeiraPrevisao).toContainText('5');
    });

    test('deve exibir o botão "Ver mais" para cada obra', async ({ page }) => {
      const botoes = page.locator('[data-testid="obra-ver-mais"]');
      await expect(botoes).toHaveCount(10);
    });

    test.describe('Controles de paginação (requer backend)', () => {
      test('deve exibir o seletor de pageSize', async ({ page }) => {
        await expect(page.locator('[data-testid="page-size-select"]')).toBeVisible();
      });

      test('deve exibir opções de pageSize 10, 25, 50, 100', async ({ page }) => {
        const select = page.locator('[data-testid="page-size-select"]');
        await expect(select).toBeVisible();
        const opcoes = page.locator('[data-testid="page-size-option"]');
        await expect(opcoes).toHaveCount(4);
      });

      test('deve exibir botões de navegação (anterior e próxima)', async ({ page }) => {
        await expect(page.locator('[data-testid="btn-anterior"]')).toBeVisible();
        await expect(page.locator('[data-testid="btn-proximo"]')).toBeVisible();
      });

      test('deve desabilitar o botão anterior na primeira página', async ({ page }) => {
        await expect(page.locator('[data-testid="btn-anterior"]')).toBeDisabled();
      });

      test('deve exibir total de registros', async ({ page }) => {
        const total = page.locator('[data-testid="paginacao-total"]');
        await expect(total).toBeVisible();
        await expect(total).toContainText('12');
      });

      test('deve habilitar botão próxima quando há mais páginas', async ({ page }) => {
        await expect(page.locator('[data-testid="btn-proximo"]')).toBeEnabled();
      });

      test('deve exibir 3 itens ao selecionar pageSize 25 (total < 25)', async ({ page }) => {
        const select = page.locator('[data-testid="page-size-select"]');
        await select.selectOption('25');
        const nomes = page.locator('[data-testid="obra-nome"]');
        await expect(nomes).toHaveCount(12);
      });
    });
  });

  test.describe('Atualizar posição em popup (demanda 1)', () => {
    test('deve exibir o botão "Atualizar posição" na página de obras', async ({ page }) => {
      await expect(page.locator('[data-testid="link-atualizar-posicao"]')).toBeVisible();
      await expect(page.locator('[data-testid="link-atualizar-posicao"]')).toContainText(
        'Atualizar posição'
      );
    });

    test('ao clicar em Atualizar posição deve abrir o dialog com o formulário (overlay e campo identificador)', async ({
      page,
    }) => {
      await page.locator('[data-testid="link-atualizar-posicao"]').click();
      await expect(page.locator('[data-testid="dialog-overlay"]')).toBeVisible();
      await expect(page.locator('[data-testid="atualizar-posicao-identificador"]')).toBeVisible();
      await expect(page).toHaveURL(/\/obras/);
    });

    test('ao fechar o dialog a lista permanece visível e a URL continua /obras', async ({
      page,
    }) => {
      await page.locator('[data-testid="link-atualizar-posicao"]').click();
      await expect(page.locator('[data-testid="dialog-overlay"]')).toBeVisible();
      await page.locator('[data-testid="atualizar-posicao-fechar"]').click();
      await expect(page.locator('[data-testid="dialog-overlay"]')).not.toBeVisible();
      await expect(page).toHaveURL(/\/obras/);
      await expect(page.locator('h2')).toContainText('Acompanhamento de Obras');
    });

    test('Demanda 2: não deve exibir o checkbox "Criar obra se não existir" no formulário de atualizar posição', async ({
      page,
    }) => {
      await page.locator('[data-testid="link-atualizar-posicao"]').click();
      await expect(page.locator('[data-testid="dialog-overlay"]')).toBeVisible();
      await expect(page.locator('[data-testid="atualizar-posicao-criar"]')).not.toBeVisible();
    });
  });
});
