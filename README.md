openapi: 3.0.0
info:
  title: Task Management API
  version: 1.0.0
paths:
  /api/tasks:
    get:
      summary: Get all tasks
      responses:
        '200':
          description: Successful response
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/TaskModel'
    post:
      summary: Create a new task
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/TaskCreateModel'
      responses:
        '201':
          description: Task created successfully
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/RequestJsonData'
        '400':
          description: Bad request
        '500':
          description: Internal server error
  /api/tasks/{taskId}:
    get:
      summary: Get a task by ID
      parameters:
        - name: taskId
          in: path
          required: true
          schema:
            type: integer
            minimum: 1
      responses:
        '200':
          description: Successful response
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/TaskModel'
        '400':
          description: Bad request
        '404':
          description: Task not found
        '500':
          description: Internal server error
    put:
      summary: Update a task by ID
      parameters:
        - name: taskId
          in: query
          required: true
          schema:
            type: integer
            minimum: 1
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/TaskUpdateModel'
      responses:
        '204':
          description: No content
        '400':
          description: Bad request
        '404':
          description: Task not found
        '500':
          description: Internal server error
    delete:
      summary: Delete a task by ID
      parameters:
        - name: taskId
          in: query
          required: true
          schema:
            type: integer
            minimum: 1
      responses:
        '204':
          description: No content
        '404':
          description: Task not found
        '500':
          description: Internal server error
  /api/tasks/assign:
    post:
      summary: Assign a task to an assignee
      parameters:
        - name: taskId
          in: query
          required: true
          schema:
            type: integer
            minimum: 1
        - name: assigneeId
          in: query
          required: true
          schema:
            type: integer
            minimum: 1
      responses:
        '204':
          description: Task assigned successfully
        '404':
          description: Task or assignee not found
        '500':
          description: Internal server error
  /api/tasks/generateReport:
    get:
      summary: Generate a report of tasks
      responses:
        '200':
          description: Successful response
          content:
            application/vnd.openxmlformats-officedocument.spreadsheetml.sheet:
              schema:
                type: string
                format: binary
        '500':
          description: Internal server error
components:
  schemas:
    TaskModel:
      type: object
      properties:
        Title:
          type: string
        Description:
          type: string
        AssigneeName:
          type: string
        StatusName:
          type: string
    TaskCreateModel:
      type: object
      properties:
        Title:
          type: string
        Description:
          type: string
        Assignee:
          type: integer
        DueDate:
          type: object
        Status:
          type: integer
    TaskUpdateModel:
      type: object
      properties:
        Id:
          type: integer
        Title:
          type: string
        Description:
          type: string
        DueDate:
          type: object
        Status:
          type: integer
    RequestJsonData:
      type: object
      properties:
        Data:
          type: object
        Message:
          type: string
        Status:
          type: integer
        Title:
          type: string 

