window.scrollToBottom = (element) => {
    element.scrollTop = element.scrollHeight;

    // find all elements with the attribute OnClickDoCommand that do not have an onclick handler
    let elements = document.querySelectorAll('[OnClickDoCommand]:not([onclick])');
};
window.addStyles = (css) => {
    let style = document.createElement('style');
    style.id = 'dynamic-styles';
    style.type = 'text/css';
    style.innerHTML = css;

    // Remove the previous styles
    let previousStyles = document.getElementById('dynamic-styles');
    if (previousStyles) {
        previousStyles.remove();
    }
    document.head.appendChild(style);
};

window.addClickEvents = () => {
    // find all elements with the attribute OnClickDoCommand
    let elements = document.querySelectorAll('[oncommand]');
    elements.forEach((element) => {
        element.onclick = () => {
            let command = element.getAttribute('oncommand');
            DotNet.invokeMethodAsync('BlazorApp1', 'SendCommandToServer', command);
        };
    });
};