// .agents/skills → .claude/skills（Windows junction，不需系統管理員權限）
import { lstatSync, mkdirSync, symlinkSync, unlinkSync } from 'node:fs';
import { resolve } from 'node:path';

const sourceDirectory = resolve('.agents/skills');
const linkPath = resolve('.claude/skills');

const existing = lstatSync(linkPath, { throwIfNoEntry: false });
if (existing?.isSymbolicLink()) {
    // 只移除連結本身，不會動到 .agents/skills 的內容
    unlinkSync(linkPath);
} else if (existing) {
    throw new Error(`${linkPath} 是實體資料夾而非連結，請確認內容後手動刪除再執行。`);
}

mkdirSync(resolve('.claude'), { recursive: true });
symlinkSync(sourceDirectory, linkPath, 'junction');
console.log(`${linkPath} → ${sourceDirectory}`);
