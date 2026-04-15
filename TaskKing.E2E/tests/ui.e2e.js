import { Selector } from "testcafe";

fixture("TaskKing UI smoke")
    .page(process.env.BASE_URL);

test("UI loads and renders task list", async t => {

    const taskList = Selector('#tasks');
    const form = Selector('#taskForm');
    const input = Selector('#taskTitle');

    await t.expect(form.exists).ok();
    await t.expect(input.exists).ok();
    await t.expect(taskList.exists).ok();

    await t.expect(Selector('body').exists).ok();

    const anyTask = taskList.find('[data-test="task-item"]');
    await t.expect(anyTask.exists).ok({ timeout: 15000 });
});