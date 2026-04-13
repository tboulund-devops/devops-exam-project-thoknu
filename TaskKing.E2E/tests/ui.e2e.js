import { Selector } from "testcafe";

const baseUrl = process.env.BASE_URL;

fixture("TaskKing UI E2E")
    .page(baseUrl);

test("create, edit and delete task", async t => {

    const input = Selector('#taskTitle');
    const form = Selector('#taskForm');
    const list = Selector('#tasks');

    const title = `ui-task-${Date.now()}`;
    const updatedTitle = `${title}-updated`;

    await t
        .typeText(input, title)
        .click(form.find('button'));

    const createdTask = list.find('li').withText(title);
    await t.expect(createdTask.exists).ok({ timeout: 5000 });

    await t.setNativeDialogHandler(() => updatedTitle);

    await t.click(createdTask.find('button').withText('Edit'));

    const updatedTask = list.find('li').withText(updatedTitle);
    await t.expect(updatedTask.exists).ok({ timeout: 5000 });

    await t.setNativeDialogHandler(null);

    await t.click(updatedTask.find('button').withText('Delete'));

    await t.expect(updatedTask.exists).notOk({ timeout: 5000 });
});