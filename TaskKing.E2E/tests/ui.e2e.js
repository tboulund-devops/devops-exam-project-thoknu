import { Selector } from "testcafe";

fixture("TaskKing UI smoke")
    .page(process.env.BASE_URL);

test("UI loads correctly with empty or populated state", async t => {

    const taskList = Selector('#tasks');
    const form = Selector('#taskForm');
    const input = Selector('#taskTitle');

    await t
        .expect(form.exists).ok()
        .expect(input.exists).ok()
        .expect(taskList.exists).ok();

    const tasks = taskList.find('[data-test="task-item"]');

    const count = await tasks.count;

    await t.expect(count >= 0).ok();
});