# User registration

This feature is to support the registration of the new users in the system. 

Issuse for reference: [#287](https://github.com/gritcsenko/HomeInventory/issues/287)

## Overview

In order to have any users in the system after initial deployment we need a way to add new users.

### API

User registration will use HTTP JSON API.

- Endpoint: `POST /users/manage/register`
- Body:

    ```json
    {
        "Email": "e-mail@server.org",
        "Password": "Some password in plain text"
    }
    ```
