# Instructions

### Demo 1 Improving the landing page with a Jumbotron Header

- Open the solution or project with your preferred IDE
- Verify that the latest version of Bootstrap is installed in your project
    - Open Solution Explorer
    - Navigate to wwwroot/lib/bootstrap/dist/css
    - Open Bootstrap.css and verify the version number in the header, e.g. Bootstrap v5.1.0
- Refer to the w3schools Bootstrap tutorial page for the version installed in your project
    - Open a browser and navigate to https://www.w3schools.com/bootstrap/bootstrap_ver.asp
- We can make use of some of the templates in this tutorial page
    - On the left side menu, find Jumbotron
    - Read tutorial explanation
    - Copy example code
- Back to the project, open /Views/Home/Index.cshtml
    - Paste the Jumbotron code inside the main page \<div>
    - Modify the title in the Jumbotron with a welcome message
    - Describe your restaurant in one sentence in the paragraph element
    - Modify the original Welcome message title to read 'Our Story'

### Demo 2 Improving the landing page with a bootstrap content grid

- Back to the tutorial page, look for these two sections and examine the corresponding examples:
    - Images
    - Grid System
- Back to /Views/Home/Index.cshtml
    - Add a div with class container right after the Jumbotron element
    - Inside of this container div, add two row divs with two columns each
    - Move the 'story' section (title and text) to the second column from left to right in the first row
    - Move the 'chef' section (title and text) to the first column from left to right in the second column
- It's time to look for royalty free images to use in our website
    - Open a browser and navigate to https://pixabay.com/
    - Search for two images that you can use to represent 'story' and 'chef'
    - Choose the commended size of 1280×960
    - Download them and copy them in /wwwroot/imgs
- Back to /Views/Home/Index.cshtml
    - Add an image element with class img-thumbnail to each empty column in each row
    - Link the image element to corresponding image

### Demo 3 Improving the privacy page with placeholder text and collapsible elements

- A common aesthetic improvement when creating projects from scratch or demo projects involves using Lorem Ipsum text
    - This is just a place holder to show how a website would look like with content
    - Open a browser and navigate to https://loremipsum.io/
    - Generate 5 paragraphs of Lorem Ipsum text
    - Copy all text
- Back to the project, open /Views/Home/Privacy.cshtml
    - Paste the generated text inside the paragraph element
- Save your changes, run the project and verify.
    - What can be improved? It's a lot of text so maybe collapsible elements
- Back to the Bootstrap tutorial page, search for Collapse
    - Scroll down to the Accordion element, read the example and try it out yourself
    - Copy the accordion code
- Back to /Views/Home/Privacy.cshtml
    - Paste the accordion code
    - Replicate the div with class card two more times, we need 5 collapsible elements
    - Modify the IDs and HREFs values of divs 4 and 5 matching the pattern accordingly
        - collapseFour
        - collapseFive
    - Move each paragraph from the \<p> element into each collapsible div with class card-body
    - Modify the text accordingly to simulate a Privacy Policy
        - Privacy Considerations
        - Data Protection
        - Data Collection
        - Data Use
        - Other Considerations1