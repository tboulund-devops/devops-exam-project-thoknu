import { Selector } from "testcafe";

const baseUrl = process.env.BASE_URL;

fixture("TaskKing UI E2E")
    .page(baseUrl);

test("create task and verify it appears in list", async t => {

    const input = Selector('#taskTitle');
    const form = Selector('#taskForm');
    const list = Selector('#tasks');

    const title = `ui-task-${Date.now()}`;

    await t
        .typeText(input, title)
        .click(form.find('button'));

    const taskItem = list.find('li').withText(title);

    await t.expect(taskItem.exists).ok({ timeout: 5000 });
});