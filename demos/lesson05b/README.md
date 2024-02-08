# Instructions

### Part 1 Adding a Contact Form
- Views are returned as the result of executing an Action Method from a controller class
- Open /Controllers/HomeController.cs
    - Add a new action method named ContactUs
    - Make it a public method
    - Set return type as IActionResult
    - No parameters
    - Returns View()
- For each action result that returns a view in a controller, we need a View file inside folder with the same name as the controller in /Views
    - Right click on /Views/Home
    - Select Add > View
    - Select Razor View
    - Set view name as the same name as the method
    - Set template to 'Empty (without model)'
    - Make sure 'Use a layout page' option is selected
- To verify, run the application and navigate to https://localhost:1234/home/contactus
- Back to /Views/Home/ContactUs.cshtml
    - Fix title and add descriptive text with instructions about the form
    - Add a form element with action set to post and inside this element:
        - Add 3 fieldset elements
        - Inside each fieldset add a label and an input elements
        - Set the css class for each label to form-label
        - Set the css class for each input to form-control
        - Set the id's of each input element to name, email, and message
        - Make the last input element a textarea
        - Add input type of email to email field
        - Add labels accordingly
    - Add a button element with class 'btn btn-primary' and with text value of Submit
- To verify, run the application and navigate to https://localhost:1234/home/contactus

### Part 2 Adding an About Us Page
- Open /Controllers/HomeController.cs
    - Add a new action method named AboutUs
    - Make it a public method
    - Set return type as IActionResult
    - No parameters
    - Returns View()
- For each action result that returns a view in a controller, we need a View file inside folder with the same name as the controller in /Views
    - Right click on /Views/Home
    - Select Add > View
    - Select Razor View
    - Set view name as the same name as the method
    - Set template to 'Empty (without model)'
    - Make sure 'Use a layout page' option is selected
- To verify, run the application and navigate to https://localhost:1234/home/aboutus
- Back to /Views/Home/AboutUs.cshtml
    - Fix the title with the name of the chef
    - Add a container div
    - Inside of this, add a row div with two columns
    - Bring over the content about the Chef from Index.cshtml
- To verify, run the application and navigate to https://localhost:1234/home/aboutus

### Part 3 Adding new navigation options
- We want to add new options to the navigation bar and apply them globally
- Open /Views/Shared/_Layout.cshtml
    - Look for the \<li> elements with class 'nav-item'
    - Add two more of those including the \<a> element
    - Use tag helpers to point the \<a> element to the corresponding controller (Home) and methods (AboutUs, ContactUs)
    - Modify the order of this elements from top to bottom:
        - Home
        - About Us
        - ContactUs
        - Privacy
- To verify, run the application and make sure the new options appear and that you can navigate to the corresponding page