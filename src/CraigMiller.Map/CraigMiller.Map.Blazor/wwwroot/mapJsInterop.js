export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}

export function fitCanvasToContainer(canvasId) {
    const canvas = document.getElementById(canvasId);

    canvas.style.width = '100%';
    canvas.style.height = '100%';

    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;
}

// From https://github.com/Mapsui/Mapsui/issues/2042#issuecomment-1584980382
export function disableMousewheelScroll(elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        element.addEventListener('wheel', e => e.preventDefault(), { passive: false });
    }
};
