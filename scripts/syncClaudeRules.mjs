// .cursor/rules/**/*.mdc → .claude/rules/**/*.md
// alwaysApply: true 或沒有 globs → 常駐載入（不寫 paths）；有 globs → 轉成 Claude 的 paths。
import { mkdirSync, readFileSync, readdirSync, rmSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';

const sourceDirectory = '.cursor/rules';
const targetDirectory = '.claude/rules';

// 整個重建，來源刪掉的規則才會一起消失
rmSync(targetDirectory, { recursive: true, force: true });

for (const relativePath of readdirSync(sourceDirectory, { recursive: true })) {
    if (!relativePath.endsWith('.mdc')) continue;

    const content = readFileSync(join(sourceDirectory, relativePath), 'utf8');
    const frontmatterMatch = content.match(/^---\r?\n([\s\S]*?)\r?\n---\r?\n?/);
    const frontmatter = frontmatterMatch?.[1] ?? '';
    const body = frontmatterMatch ? content.slice(frontmatterMatch[0].length) : content;

    const isAlwaysApply = /^alwaysApply:\s*true\s*$/m.test(frontmatter);
    const globs = (frontmatter.match(/^globs:\s*(.+)$/m)?.[1] ?? '')
        .split(',')
        .map((glob) => glob.trim())
        .filter(Boolean);

    const claudeFrontmatter =
        isAlwaysApply || globs.length === 0
            ? ''
            : `---\npaths:\n${globs.map((glob) => `  - "${glob}"`).join('\n')}\n---\n\n`;

    const targetPath = join(targetDirectory, relativePath.replace(/\.mdc$/, '.md'));
    mkdirSync(dirname(targetPath), { recursive: true });
    writeFileSync(targetPath, claudeFrontmatter + body.trimStart(), 'utf8');

    console.log(`${relativePath} → ${targetPath}${globs.length && !isAlwaysApply ? ` (paths: ${globs.join(', ')})` : ' (常駐)'}`);
}
