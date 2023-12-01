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
