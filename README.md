
# Title: Task Management API
Version: 1.0.0

# Endpoints:
# GET /api/tasks:

Summary: Get all tasks

 Responses:

200: Successful response (array of TaskModel)
400: Bad request
500: Internal server error
# POST /api/tasks:

Summary: Create a new task
Request Body: TaskCreateModel
Responses:
201: Task created successfully (RequestJsonData)
400: Bad request
500: Internal server error
# GET /api/tasks/{taskId}:

Summary: Get a task by ID

Parameters: taskId (integer)

Responses:

200: Successful response (TaskModel)
400: Bad request
404: Task not found
500: Internal server error
# PUT /api/tasks/{taskId}:

Summary: Update a task by ID
Parameters: taskId (integer, in query)
Request Body: TaskUpdateModel
Responses:
204: No content
400: Bad request
404: Task not found
500: Internal server error
# DELETE /api/tasks/{taskId}:

Summary: Delete a task by ID
Parameters: taskId (integer, in query)
Responses:
204: No content
404: Task not found
500: Internal server error
# POST /api/tasks/assign:

Summary: Assign a task to an assignee
Parameters: taskId, assigneeId (both integers, in query)
Responses:
204: Task assigned successfully
404: Task or assignee not found
500: Internal server error
# GET /api/tasks/generateReport:

Summary: Generate a report of tasks
Responses:
200: Successful response (binary data - spreadsheet)
500: Internal server error
# Components:
# Schemas:
TaskModel: Describes properties of a task
TaskCreateModel: Describes properties for creating a task
TaskUpdateModel: Describes properties for updating a task
RequestJsonData: Describes JSON data structure for responses
