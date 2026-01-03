const puppeteer = require('puppeteer');
const path = require('path');
const fs = require('fs');

const wireframesDir = path.join(__dirname);
const screenshotsDir = path.join(__dirname, 'screenshots');

const wireframes = [
  { file: 'login-signup.html', name: 'login', modes: ['light'] },
  { file: 'dashboard.html', name: 'dashboard', modes: ['light', 'dark'] },
  { file: 'board-view.html', name: 'board-view', modes: ['light', 'dark'] },
  { file: 'task-list-view.html', name: 'task-list-view', modes: ['light'] },
  { file: 'calendar-view.html', name: 'calendar-view', modes: ['light'] },
  { file: 'gantt-view.html', name: 'gantt-view', modes: ['light'] },
  { file: 'task-detail-modal.html', name: 'task-detail-modal', modes: ['light', 'dark'] },
  { file: 'settings-page.html', name: 'settings-page', modes: ['light'] },
  { file: 'team-members-page.html', name: 'team-members-page', modes: ['light'] },
  { file: 'ai-chatbot.html', name: 'ai-chatbot', modes: ['light', 'dark'] },
];

async function captureScreenshots() {
  const browser = await puppeteer.launch({
    headless: 'new',
    args: ['--no-sandbox', '--disable-setuid-sandbox']
  });

  console.log('🎨 Capturing screenshots...\n');

  for (const wireframe of wireframes) {
    const filePath = `file://${path.join(wireframesDir, wireframe.file)}`;

    for (const mode of wireframe.modes) {
      const page = await browser.newPage();

      // Set viewport
      await page.setViewport({ width: 1920, height: 1080 });

      // Navigate to wireframe
      await page.goto(filePath, { waitUntil: 'networkidle0' });

      // Wait a bit for any animations
      await new Promise(resolve => setTimeout(resolve, 500));

      // Toggle dark mode if needed
      if (mode === 'dark') {
        await page.evaluate(() => {
          const toggle = document.querySelector('[data-theme-toggle]');
          if (toggle) toggle.click();
        });
        await new Promise(resolve => setTimeout(resolve, 500));
      }

      // Capture screenshot
      const outputPath = path.join(screenshotsDir, mode, `${wireframe.name}-${mode}.png`);
      await page.screenshot({
        path: outputPath,
        fullPage: true
      });

      console.log(`✅ ${wireframe.name}-${mode}.png`);

      await page.close();
    }
  }

  await browser.close();
  console.log('\n✨ All screenshots captured!');
  console.log(`📁 Location: ${screenshotsDir}`);
}

captureScreenshots().catch(console.error);
