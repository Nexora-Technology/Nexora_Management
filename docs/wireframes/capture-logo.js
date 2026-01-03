const puppeteer = require('puppeteer');
const path = require('path');

const wireframesDir = path.join(__dirname);
const screenshotsDir = path.join(__dirname, 'screenshots', 'logo');

async function captureLogo() {
  const browser = await puppeteer.launch({
    headless: 'new',
    args: ['--no-sandbox', '--disable-setuid-sandbox']
  });

  console.log('🎨 Capturing logo screenshots...\n');

  // Create simple HTML page with logo
  const html = '<!DOCTYPE html><html><head><style>body{margin:0;padding:50px;display:flex;flex-direction:column;gap:30px;background:transparent}.logo-container{background:white;padding:30px;border-radius:12px;display:flex;justify-content:center;align-items:center}.logo-dark{background:#1e293b}</style></head><body><div class="logo-container"><img src="logo.svg" width="400" height="100" alt="Nexora Logo"></div><div class="logo-container logo-dark"><img src="logo.svg" width="400" height="100" alt="Nexora Logo Dark"></div><div class="logo-container"><img src="logo-icon.svg" width="100" height="100" alt="Nexora Icon"></div></body></html>';

  const page = await browser.newPage();
  await page.setViewport({ width: 800, height: 600 });
  await page.setContent(html, { waitUntil: 'networkidle0' });

  // Capture full page
  await page.screenshot({
    path: path.join(screenshotsDir, 'logos.png'),
    fullPage: true
  });

  console.log('✅ logos.png captured');

  await browser.close();
  console.log('\n✨ Logo screenshots captured!');
  console.log('📁 Location: ' + screenshotsDir);
}

captureLogo().catch(console.error);
