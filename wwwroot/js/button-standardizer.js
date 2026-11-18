/**
 * button-standardizer.js
 * 
 * Este script encuentra y reubica botones de "Regresar" o "Volver a la Lista" que estén al final 
 * de la página y los coloca justo después del título h4, con un estilo consistente.
 * También elimina las líneas horizontales (hr) que aparecen después de los títulos de sección.
 */
// Variable global para controlar si el script ya se ejecutó
let standardizationApplied = false;

document.addEventListener('DOMContentLoaded', function() {
    // Esperar a que el DOM esté completamente cargado
    if (!standardizationApplied) {
        setTimeout(standardizeButtons, 100);
        standardizationApplied = true;
    }
});

function standardizeButtons() {
    // Verificar si ya hay botones duplicados y eliminarlos primero
    cleanupDuplicateButtons();
    
    // Buscar h4 en la página - suele ser el título de sección
    const h4Elements = document.querySelectorAll('h4');
    if (!h4Elements.length) return;
    
    // Tomar solo el primer h4 para evitar duplicados
    const mainH4 = h4Elements[0];
    
    // Verificar si ya hay un botón de regreso después del h4
    let nextElement = mainH4.nextElementSibling;
    
    // Eliminar línea horizontal si existe
    if (nextElement && nextElement.tagName.toLowerCase() === 'hr') {
        nextElement.remove();
        nextElement = mainH4.nextElementSibling;
    }
    
    // Si ya hay un botón de regreso, no hacer nada más
    if (nextElement && isBackButton(nextElement)) {
        // Asegurarse de que tenga el estilo correcto
        const existingButton = nextElement.tagName.toLowerCase() === 'a' ? nextElement : nextElement.querySelector('a');
        if (existingButton && !existingButton.classList.contains('btn')) {
            convertToStyledButton(existingButton);
        }
        return;
    }
    
    // Buscar un botón de regreso al final de la página
    const backButton = findSingleBackButton();
    
    if (backButton) {
        // Si encontramos un botón, moverlo después del h4
        moveButtonAfterHeading(backButton, mainH4);
    } else {
        // Si no hay botón, crear uno nuevo
        createNewBackButton(mainH4);
    }
}

// Limpia botones duplicados que pudieran haberse creado
function cleanupDuplicateButtons() {
    // Buscar todos los botones de regreso
    const backButtons = document.querySelectorAll('a[asp-action="Index"]');
    const backButtonsArray = Array.from(backButtons);
    
    // Crear un mapa para agrupar por controlador
    const buttonsByController = {};
    
    backButtonsArray.forEach(button => {
        const controller = button.getAttribute('asp-controller') || 'default';
        if (!buttonsByController[controller]) {
            buttonsByController[controller] = [];
        }
        buttonsByController[controller].push(button);
    });
    
    // Para cada controlador, mantener solo un botón
    Object.keys(buttonsByController).forEach(controller => {
        const buttons = buttonsByController[controller];
        if (buttons.length > 1) {
            // Mantener el primer botón (posiblemente el que esté más cerca del título)
            const h4 = document.querySelector('h4');
            if (h4) {
                // Ver cuál está más cerca del h4
                let closestButton = null;
                let closestDistance = Infinity;
                
                buttons.forEach(button => {
                    // Calcular si el botón está cerca del h4
                    const isNextToH4 = button.previousElementSibling === h4 || 
                                       (button.parentElement && button.parentElement.previousElementSibling === h4);
                    
                    if (isNextToH4) {
                        closestButton = button;
                        closestDistance = 0;
                    }
                });
                
                // Si encontramos un botón cerca del h4, mantenerlo, eliminar los demás
                if (closestButton) {
                    buttons.forEach(button => {
                        if (button !== closestButton) {
                            try {
                                if (button.parentElement && button.parentElement.children.length === 1) {
                                    button.parentElement.remove();
                                } else {
                                    button.remove();
                                }
                            } catch (e) {
                                console.log('Error al eliminar botón duplicado:', e);
                            }
                        }
                    });
                } else {
                    // Si no hay botón cerca del h4, mantener el primero, eliminar los demás
                    for (let i = 1; i < buttons.length; i++) {
                        try {
                            if (buttons[i].parentElement && buttons[i].parentElement.children.length === 1) {
                                buttons[i].parentElement.remove();
                            } else {
                                buttons[i].remove();
                            }
                        } catch (e) {
                            console.log('Error al eliminar botón duplicado:', e);
                        }
                    }
                }
            }
        }
    });
}

// Encuentra un solo botón de regreso (preferiblemente al final de la página)
function findSingleBackButton() {
    // Buscar todos los botones de regreso
    const allButtons = findAllBackButtons();
    
    if (allButtons.length === 0) return null;
    
    // Preferir botones que estén al final del documento
    // ordena los botones por su posición en el documento
    allButtons.sort((a, b) => {
        const posA = a.getBoundingClientRect().top;
        const posB = b.getBoundingClientRect().top;
        return posB - posA; // orden descendente (los que están más abajo primero)
    });
    
    return allButtons[0];
}

// Mueve un botón después de un encabezado
function moveButtonAfterHeading(button, heading) {
    // Determinar el botón real (puede estar dentro de un contenedor)
    const buttonElement = button.tagName.toLowerCase() === 'a' ? button : button.querySelector('a');
    if (!buttonElement) return;
    
    // Si es un enlace simple, convertirlo a botón estilizado
    if (isSimpleLink(buttonElement)) {
        convertToStyledButton(buttonElement);
    }
    
    // Crear una copia del botón
    const buttonClone = buttonElement.cloneNode(true);
    
    // Asegurarse de que tenga el estilo correcto
    if (!buttonClone.classList.contains('btn')) {
        convertToStyledButton(buttonClone);
    }
    
    // Insertar después del encabezado
    heading.insertAdjacentElement('afterend', buttonClone);
    
    // Eliminar el original
    try {
        if (button !== buttonElement && button.children.length === 1) {
            // Es un contenedor con solo el botón, eliminar el contenedor
            button.parentNode.removeChild(button);
        } else if (button === buttonElement) {
            // Es el botón directamente
            buttonElement.parentNode.removeChild(buttonElement);
        } else {
            // Es un contenedor con más elementos, eliminar solo el botón
            buttonElement.parentNode.removeChild(buttonElement);
        }
    } catch (e) {
        console.log('Error al eliminar botón original:', e);
    }
}

function isBackButton(element) {
    // Verificar si el elemento es un botón de regreso o contiene uno
    if (element.tagName.toLowerCase() === 'a') {
        const text = element.textContent.trim().toLowerCase();
        return text.includes('regresar') || text.includes('volver');
    } else if (element.querySelector) {
        const link = element.querySelector('a');
        return link && (
            link.textContent.trim().toLowerCase().includes('regresar') || 
            link.textContent.trim().toLowerCase().includes('volver')
        );
    }
    return false;
}

function findAllBackButtons() {
    // Encontrar todos los posibles botones de regreso en la página
    const backButtons = [];
    
    // Buscar enlaces directos
    document.querySelectorAll('a').forEach(link => {
        const text = link.textContent.trim().toLowerCase();
        if (text.includes('regresar') || text.includes('volver a la lista')) {
            backButtons.push(link);
        }
    });
    
    // Buscar contenedores que contienen enlaces de regreso (divs)
    document.querySelectorAll('div').forEach(div => {
        // Considerar todos los divs que contienen un enlace con texto específico
        const link = div.querySelector('a');
        if (link && (
            link.textContent.trim().toLowerCase().includes('regresar') || 
            link.textContent.trim().toLowerCase().includes('volver a la lista')
        )) {
            // Si el enlace ya fue añadido directamente, no añadir el div
            if (!backButtons.includes(link)) {
                // Si el div parece ser un contenedor de botón (pocos elementos y sólo texto simple)
                if (div.children.length <= 2 || div.textContent.trim().length < 50) {
                    backButtons.push(div);
                } else {
                    backButtons.push(link);
                }
            }
        }
    });
    
    // Buscar enlaces que apuntan al índice (típicamente botones de regreso)
    document.querySelectorAll('a[asp-action="Index"]').forEach(link => {
        if (!backButtons.includes(link)) {
            backButtons.push(link);
        }
    });
    
    return backButtons;
}

// Para mantener compatibilidad
function findBackButtons() {
    return findAllBackButtons();
}

// Función para crear un nuevo botón de regreso cuando no se encuentra ninguno
function createNewBackButton(h4Element) {
    if (!h4Element) return;
    
    // Determinar el controlador basado en la URL
    const path = window.location.pathname.split('/');
    let controller = 'Home';
    if (path.length > 1 && path[1]) {
        controller = path[1];
    }
    
    // Crear el nuevo botón
    const newButton = document.createElement('a');
    newButton.className = 'btn d-inline-flex align-items-center gap-2';
    newButton.setAttribute('style', 'background-color: #1e386a; color: white; padding: 8px 16px; border-radius: 4px; text-decoration: none; transition: opacity 0.2s;');
    newButton.setAttribute('asp-action', 'Index');
    newButton.setAttribute('asp-controller', controller);
    newButton.href = `/${controller}/Index`;
    
    newButton.innerHTML = `
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
            <path fill-rule="evenodd" d="M15 8a.5.5 0 0 0-.5-.5H2.707l3.147-3.146a.5.5 0 1 0-.708-.708l-4 4a.5.5 0 0 0 0 .708l4 4a.5.5 0 0 0 .708-.708L2.707 8.5H14.5A.5.5 0 0 0 15 8z" />
        </svg>
        Regresar
    `;
    
    // Insertar después del h4
    h4Element.insertAdjacentElement('afterend', newButton);
}

function isSimpleLink(element) {
    // Verificar si es un enlace simple sin estilos de botón
    if (element.tagName.toLowerCase() !== 'a') return false;
    
    const hasButtonClass = element.classList.contains('btn');
    const hasInlineStyles = element.getAttribute('style') && 
                           element.getAttribute('style').includes('background-color');
    
    return !hasButtonClass && !hasInlineStyles;
}

function convertToStyledButton(link) {
    // Convertir un enlace simple a un botón estilizado
    const controller = link.getAttribute('asp-controller') || 
                      (link.href ? new URL(link.href).pathname.split('/')[1] : '');
    
    const action = link.getAttribute('asp-action') || 'Index';
    
    // Mantener el href original si existe
    const href = link.href;
    
    // Texto del botón
    const buttonText = link.textContent.trim();
    
    // Crear botón estilizado
    link.setAttribute('class', 'btn d-inline-flex align-items-center gap-2');
    link.setAttribute('style', 'background-color: #1e386a; color: white; padding: 8px 16px; border-radius: 4px; text-decoration: none; transition: opacity 0.2s;');
    
    // Asegurarse de que asp-controller y asp-action estén presentes
    if (controller && !link.hasAttribute('asp-controller')) {
        link.setAttribute('asp-controller', controller);
    }
    
    if (!link.hasAttribute('asp-action')) {
        link.setAttribute('asp-action', action);
    }
    
    // Agregar ícono SVG
    link.innerHTML = `
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
            <path fill-rule="evenodd" d="M15 8a.5.5 0 0 0-.5-.5H2.707l3.147-3.146a.5.5 0 1 0-.708-.708l-4 4a.5.5 0 0 0 0 .708l4 4a.5.5 0 0 0 .708-.708L2.707 8.5H14.5A.5.5 0 0 0 15 8z" />
        </svg>
        ${buttonText === 'Volver a la Lista' ? 'Regresar' : buttonText}
    `;
    
    // Restaurar href si existía
    if (href) {
        link.href = href;
    }
}
