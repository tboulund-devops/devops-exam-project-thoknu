import { Selector } from "testcafe";

const baseUrl = process.env.BASE_URL;

fixture("TaskKing UI E2E")
    .page(baseUrl)
    .beforeEach(async t => {
        await t.eval(() => localStorage.clear());
    });

test("create and delete task", async t => {

    const input = Selector('#taskTitle');
    const form = Selector('#taskForm');
    const taskList = Selector('#tasks');

    const title = `ui-task-${Date.now()}`;

    await t
        .typeText(input, title)
        .click(form.find('button'));

    await t.expect(taskList.find('[data-test="task-item"]').exists).ok({ timeout: 15000 });

    const createdTask = taskList.find('[data-test="task-item"]').withText(title);

    await t.expect(createdTask.exists).ok({ timeout: 15000 });

    const deleteBtn = createdTask.find('button').withText('Delete');

    await t.click(deleteBtn);

    await t.expect(createdTask.exists).notOk({ timeout: 15000 });
});