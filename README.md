This is a sample project.
This project supports only mouse inputs.

This project took 9 hours and 28 minutes to design and develop.

- TestMyStackController is the main controller class.
- In TestMyStackController.Start(), a StackApiRequest class instance used for downloading and parsing the server message.
- In TestMyStackController.Start(), Turntable class initialized which is responsible for positioning JengaBoxManagers.
- In TestMyStackController.Start(), JengaBoxManager instances are instantiated.
    - JengaBoxManagers' initialize JengaBox prefabs and configure JengaBoxController on them.
        - JengaBoxController sets material of the block and the text on the block
- In TestMyStackController.Start(), TopMenuManager is set

- In TestMyStackController.Update(), TestMyStackController.CheckClicks() method checks mouse buttons and raycasts.
    - If a JengaBoxController clicked, then it gets highlighted and infobox gets enabled with related data.