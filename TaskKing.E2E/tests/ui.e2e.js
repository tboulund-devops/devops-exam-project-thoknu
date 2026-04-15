import { Selector } from "testcafe";

fixture("TaskKing UI smoke")
    .page(process.env.BASE_URL);

test("task can be added and removed", async t => {

    const input = Selector('#taskTitle');
    const form = Selector('#taskForm');
    const taskList = Selector('#tasks');

    const title = `task-${Date.now()}`;

    await t
        .typeText(input, title)
        .click(form.find('button'));

    const anyTask = taskList.find('[data-test="task-item"]');

    await t.expect(anyTask.exists).ok({ timeout: 15000 });

    const createdTask = anyTask.withText(title);

    await t.expect(createdTask.exists).ok({ timeout: 15000 });

    await t.click(createdTask.find('button').withText('Delete'));

    await t.expect(createdTask.exists).notOk({ timeout: 15000 });
});