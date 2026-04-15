import { Selector } from "testcafe";

const baseUrl = process.env.BASE_URL;

fixture("TaskKing UI E2E")
    .page(baseUrl);

test("create and delete task", async t => {

    const input = Selector('#taskTitle');
    const form = Selector('#taskForm');

    const title = `ui-task-${Date.now()}`;

    await t
        .typeText(input, title)
        .click(form.find('button'));

    const createdTask = Selector('[data-test="task-item"]').withAttribute('data-title', title);

    await t.expect(createdTask.exists).ok({ timeout: 5000 });

    const deleteBtn = createdTask.find('button').withText('Delete');

    await t.click(deleteBtn);

    await t.expect(createdTask.exists).notOk({ timeout: 5000 });
});