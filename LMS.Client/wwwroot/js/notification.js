var usersTable;
var notificationsTable;


$(document).ready(function () {
    loadUsers();
    loadNotifications();
});

function loadUsers() {
    usersTable = $("#usersTable").DataTable({
        "ajax": {
            "url": "/Admin/Notification/GetAllUsersData",
            "dataSrc": "data"
        },
        "columns": [
            { "data": "fullName" },
            { "data": "phoneNumber" },
            { "data": "email" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                <a href="/Admin/Notification/Send/${data}" class="btn btn-warning btn-sm">Send</a>
            `
                }
            }
        ]
    });
}

function loadNotifications() {
    notificationsTable = $("#notificationsTable").DataTable({
        "ajax": {
            "url": "/Admin/Notification/GetAllNotifications",
            "dataSrc": "data"
        },
        "columns": [
            { "data": "notificationType" },
            { "data": "message" },
            { "data": "receiver.FullName" },
            {
                "data": "notificationId",
                "render": function (data) {
                    return `
                <a href="/Admin/Notification/Details/${data}" class="btn btn-warning btn-sm">Details</a>
                <a href="/Admin/Notification/Edit/${data}" class="btn btn-warning btn-sm">Edit</a>
            `
                }
            }
        ]
    });
}