/**
 * UI-Enhancements.js
 * Maneja la funcionalidad para las mejoras de la interfaz de usuario
 */
document.addEventListener('DOMContentLoaded', function() {
    // Inicializar todos los componentes (menú contextual y FAB desactivados)
    // initFabButtons(); // Desactivado por solicitud del usuario
    // initContextMenu(); // Desactivado por solicitud del usuario
    initNotifications();
    initThemeSwitcher();
});

/**
 * Botones de acción flotantes (FAB)
 */
function initFabButtons() {
    // Crear el botón flotante principal y añadirlo al DOM si no existe ya
    if (!document.querySelector('.fab-container')) {
        const fabContainer = createFabContainer();
        document.body.appendChild(fabContainer);
        
        // Añadir opciones según el contexto (controlador y acción actual)
        addContextualFabOptions(fabContainer);
    }
    
    // Manejar clics en el botón flotante principal
    document.addEventListener('click', function(e) {
        if (e.target.closest('.fab-button')) {
            const fabContainer = e.target.closest('.fab-container');
            fabContainer.classList.toggle('active');
        } else if (!e.target.closest('.fab-options')) {
            // Cerrar el menú si se hace clic fuera
            const fabContainer = document.querySelector('.fab-container');
            if (fabContainer) {
                fabContainer.classList.remove('active');
            }
        }
    });
}

function createFabContainer() {
    const fabContainer = document.createElement('div');
    fabContainer.className = 'fab-container';
    
    const fabButton = document.createElement('div');
    fabButton.className = 'fab-button';
    fabButton.innerHTML = '<i class="fas fa-plus"></i>';
    
    const fabOptions = document.createElement('div');
    fabOptions.className = 'fab-options';
    
    fabContainer.appendChild(fabOptions);
    fabContainer.appendChild(fabButton);
    
    return fabContainer;
}

function addContextualFabOptions(fabContainer) {
    const controller = getControllerName();
    const action = getActionName();
    const fabOptions = fabContainer.querySelector('.fab-options');
    
    // Limpiar opciones existentes
    fabOptions.innerHTML = '';
    
    // Opciones genéricas basadas en el controlador
    if (controller) {
        // Si estamos en una vista de índice, mostrar opción para crear
        if (action === 'Index') {
            addFabOption(fabOptions, 'Crear nuevo', 'plus', controller, 'Create');
        }
        
        // Si estamos en una vista de detalles, mostrar opciones para editar y eliminar
        if (action === 'Details') {
            const id = getEntityId();
            if (id) {
                addFabOption(fabOptions, 'Editar', 'edit', controller, 'Edit', id);
                addFabOption(fabOptions, 'Eliminar', 'trash-alt', controller, 'Delete', id);
            }
        }
        
        // Opciones específicas por controlador
        if (controller === 'Producto' || controller === 'Inventario') {
            addFabOption(fabOptions, 'Generar reporte', 'file-pdf', controller, 'Reportes');
        }
        
        if (controller === 'Factura') {
            addFabOption(fabOptions, 'Nota de crédito', 'file-invoice', controller, 'NotaCredito');
        }
    }
    
    // Si no hay opciones, ocultar el FAB
    if (fabOptions.children.length === 0) {
        fabContainer.style.display = 'none';
    } else {
        fabContainer.style.display = 'block';
    }
}

function addFabOption(fabOptions, text, icon, controller, action, id = null) {
    const option = document.createElement('a');
    option.className = 'fab-option';
    option.innerHTML = `<i class="fas fa-${icon}"></i> ${text}`;
    
    // Construir el href
    let href = `/${controller}/${action}`;
    if (id) {
        href += `/${id}`;
    }
    
    option.href = href;
    fabOptions.appendChild(option);
}

/**
 * Menú contextual
 */
function initContextMenu() {
    const mainContainer = document.querySelector('main.pb-3');
    if (!mainContainer) return;
    
    // Verificar si ya existe un menú contextual
    if (mainContainer.querySelector('.context-menu')) return;
    
    const controller = getControllerName();
    const action = getActionName();
    
    // Solo crear el menú contextual si tenemos un controlador
    if (controller) {
        const contextMenu = createContextMenu(controller, action);
        
        // Insertar al principio del contenedor principal (después de los breadcrumbs si existen)
        const breadcrumbs = mainContainer.querySelector('.custom-breadcrumb');
        if (breadcrumbs) {
            breadcrumbs.insertAdjacentElement('afterend', contextMenu);
        } else {
            mainContainer.insertAdjacentElement('afterbegin', contextMenu);
        }
    }
}

function createContextMenu(controller, action) {
    const contextMenu = document.createElement('div');
    contextMenu.className = 'context-menu';
    
    const title = document.createElement('div');
    title.className = 'context-menu-title';
    title.textContent = 'Acciones rápidas';
    
    const items = document.createElement('div');
    items.className = 'context-menu-items';
    
    // Añadir elementos del menú según el contexto
    addContextualMenuItems(items, controller, action);
    
    contextMenu.appendChild(title);
    contextMenu.appendChild(items);
    
    return contextMenu;
}

function addContextualMenuItems(itemsContainer, controller, action) {
    // Acción para volver al índice (excepto si ya estamos en el índice)
    if (action !== 'Index') {
        addContextMenuItem(itemsContainer, 'Ver lista', 'list', controller, 'Index');
    }
    
    // Acción para crear (si estamos en índice o detalles)
    if (action === 'Index' || action === 'Details') {
        addContextMenuItem(itemsContainer, 'Crear nuevo', 'plus', controller, 'Create');
    }
    
    // Acciones específicas por controlador
    if (controller === 'Producto' || controller === 'Inventario') {
        addContextMenuItem(itemsContainer, 'Categorías', 'tags', 'CategoriaProducto', 'Index');
        addContextMenuItem(itemsContainer, 'Reportes', 'chart-bar', controller, 'Reportes');
    }
    
    if (controller === 'Factura') {
        addContextMenuItem(itemsContainer, 'Clientes', 'users', 'Persona', 'Index');
        addContextMenuItem(itemsContainer, 'Productos', 'box', 'Producto', 'Index');
    }
    
    if (controller === 'Transaccion') {
        addContextMenuItem(itemsContainer, 'Tipos de cuenta', 'list-ul', 'Tipocuentum', 'Index');
    }
    
    // Si estamos en una vista de detalles o edición, añadir opciones relacionadas con el ID
    if ((action === 'Details' || action === 'Edit') && getEntityId()) {
        const id = getEntityId();
        
        if (action !== 'Edit') {
            addContextMenuItem(itemsContainer, 'Editar', 'edit', controller, 'Edit', id);
        }
        
        if (action !== 'Delete') {
            addContextMenuItem(itemsContainer, 'Eliminar', 'trash-alt', controller, 'Delete', id);
        }
    }
}

function addContextMenuItem(container, text, icon, controller, action, id = null) {
    const item = document.createElement('a');
    item.className = 'context-menu-item';
    item.innerHTML = `<i class="fas fa-${icon}"></i> ${text}`;
    
    // Construir el href
    let href = `/${controller}/${action}`;
    if (id) {
        href += `/${id}`;
    }
    
    item.href = href;
    container.appendChild(item);
}

/**
 * Sistema de notificaciones mejoradas
 */
function initNotifications() {
    // Crear el contenedor de notificaciones si no existe
    if (!document.querySelector('.notification-container')) {
        const container = document.createElement('div');
        container.className = 'notification-container';
        document.body.appendChild(container);
    }
    
    // Sobreescribir la función de SweetAlert para usar nuestro sistema
    if (window.Swal) {
        const originalSwal = window.Swal.fire;
        
        window.Swal.fire = function(options) {
            // Para confirmaciones y prompts, usar el Swal original
            if (options.showCancelButton || options.input) {
                return originalSwal.apply(this, arguments);
            }
            
            // Para notificaciones simples, usar nuestro sistema
            showNotification({
                title: options.title || '',
                message: options.text || options.html || '',
                type: options.icon || 'info'
            });
            
            // Devolver una promesa resuelta para mantener la compatibilidad
            return Promise.resolve({ isConfirmed: true, value: true });
        };
    }
}

// Función global para mostrar notificaciones
window.showNotification = function(options) {
    const container = document.querySelector('.notification-container');
    if (!container) return;
    
    const notification = document.createElement('div');
    notification.className = `notification notification-${options.type}`;
    
    notification.innerHTML = `
        <div class="notification-header">
            <span class="notification-title">
                <i class="notification-icon fas fa-${getIconForType(options.type)}"></i>
                ${options.title}
            </span>
            <button class="notification-close">
                <i class="fas fa-times"></i>
            </button>
        </div>
        <div class="notification-body">${options.message}</div>
        <div class="notification-progress"></div>
    `;
    
    container.appendChild(notification);
    
    // Manejar cierre de notificación
    const closeButton = notification.querySelector('.notification-close');
    closeButton.addEventListener('click', function() {
        closeNotification(notification);
    });
    
    // Auto-cerrar después de 5 segundos
    setTimeout(function() {
        closeNotification(notification);
    }, 5000);
};

function closeNotification(notification) {
    notification.classList.add('slide-out');
    setTimeout(function() {
        if (notification.parentNode) {
            notification.parentNode.removeChild(notification);
        }
    }, 300);
}

function getIconForType(type) {
    switch (type) {
        case 'success': return 'check-circle';
        case 'error': return 'exclamation-circle';
        case 'warning': return 'exclamation-triangle';
        case 'info':
        default: return 'info-circle';
    }
}

/**
 * Selector de tema (modo oscuro/claro)
 */
function initThemeSwitcher() {
    // Crear el botón de cambio de tema si no existe
    if (!document.querySelector('.theme-switcher')) {
        const themeSwitcher = document.createElement('div');
        themeSwitcher.className = 'theme-switcher';
        themeSwitcher.innerHTML = '<i class="fas fa-moon"></i>';
        
        themeSwitcher.addEventListener('click', function() {
            toggleDarkMode();
            
            // Actualizar el icono
            const icon = themeSwitcher.querySelector('i');
            if (document.body.classList.contains('dark-mode')) {
                icon.className = 'fas fa-sun';
            } else {
                icon.className = 'fas fa-moon';
            }
        });
        
        document.body.appendChild(themeSwitcher);
    }
    
    // Comprobar preferencia guardada
    const darkModeEnabled = localStorage.getItem('darkMode') === 'true';
    if (darkModeEnabled) {
        document.body.classList.add('dark-mode');
        document.querySelector('.theme-switcher i').className = 'fas fa-sun';
    }
}

function toggleDarkMode() {
    document.body.classList.toggle('dark-mode');
    const isDarkMode = document.body.classList.contains('dark-mode');
    localStorage.setItem('darkMode', isDarkMode);
}

/**
 * Funciones auxiliares
 */
function getControllerName() {
    // Intentar obtener el controlador de la URL
    const path = window.location.pathname.split('/');
    if (path.length > 1 && path[1]) {
        return path[1];
    }
    
    // Intentar obtener del breadcrumb
    const breadcrumbItem = document.querySelector('.breadcrumb-item:nth-child(2)');
    if (breadcrumbItem) {
        const link = breadcrumbItem.querySelector('a');
        if (link && link.getAttribute('asp-controller')) {
            return link.getAttribute('asp-controller');
        }
    }
    
    return null;
}

function getActionName() {
    // Intentar obtener la acción de la URL
    const path = window.location.pathname.split('/');
    if (path.length > 2 && path[2]) {
        return path[2];
    }
    
    // Por defecto, asumir "Index"
    return 'Index';
}

function getEntityId() {
    // Intentar obtener el ID de la URL
    const path = window.location.pathname.split('/');
    if (path.length > 3 && path[3]) {
        return path[3];
    }
    
    // Buscar en la URL como parámetro
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');
    if (id) {
        return id;
    }
    
    return null;
}
