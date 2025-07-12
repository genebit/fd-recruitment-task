# FD Test case project!

Welcome to your test task.

Our test task is based on the Clean Architecture Solution Template that uses .NET and Angular. In it, there is a simple Todo app that we wish you to add three features. After finishing the task, please create a GitHub repository containing this project's initial files in the main branch and separate branches for every feature you will work on. And make Pull Request for every feature for us to review.

## Git Workflow

**main (production) ← development (staging) ← preview (integration) ← feature branches**

> DEVNOTE: `preview` is a temporary branch for merging all features to development. This way, pull requests are visible without being merged.

## Feature 1
- Users can change the background color for each Todo item.

## Feature 2
- Users can add and remove tags to the Todo items.
- Users can filter Todo items by tags.
- Add shortcuts on UI for the most used tags by the user. (nice to have)
- Add text search. (nice to have)

## Feature 3
- Add soft delete for Todo lists and items.
- Deleted items should not be deleted from the database
- Deleted items should not be shown on the UI or included in any query

**Include as much unit test coverage as possible for the code you add.**

## How to run the Aplication

The application can be run in Visual Studio with .NET 6 as the minimum runtime requirement.

1. Load the `.sln` file under working directory.
2. Set `src/WebUI` as the startup application.
3. Restore packages.
4. Run migration. e.g., using
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/WebUI
```
5. Run the application.

## Testing

Run `dotnet test` under the working directory to view integration and unit tests.