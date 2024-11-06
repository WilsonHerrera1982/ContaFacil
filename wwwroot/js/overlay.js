const LoaderManager = {
    overlay: null,
    downloadTimeout: null,

    init() {
        this.overlay = document.getElementById('loaderOverlay');
        this.setupInterceptors();
        this.setupFormInterceptors();
        this.setupLinkInterceptors();
    },

    show() {
        if (this.overlay) {
            this.overlay.classList.add('show');
        }
    },

    hide() {
        if (this.overlay) {
            this.overlay.classList.remove('show');
        }
        if (this.downloadTimeout) {
            clearTimeout(this.downloadTimeout);
            this.downloadTimeout = null;
        }
    },

    handleDownload() {
        this.show();
        this.downloadTimeout = setTimeout(() => {
            this.hide();
        }, 3000);
    },

    setupInterceptors() {
        if (window.jQuery) {
            $(document).ajaxStart(() => this.show());
            $(document).ajaxComplete(() => this.hide());
        }
    },

    setupFormInterceptors() {
        document.addEventListener('submit', (e) => {
            if (e.target.tagName === 'FORM') {
                const isDownload = e.target.getAttribute('data-download') === 'true';
                if (isDownload) {
                    this.handleDownload();
                } else {
                    this.show();
                }
            }
        });
    },

    shouldIgnoreElement(element) {
        if (!element) return true;

        // Ignorar elementos con la clase no-loader
        if (element.classList.contains('no-loader')) return true;

        // Ignorar elementos de menús desplegables
        if (element.hasAttribute('data-bs-toggle')) return true;
        if (element.getAttribute('role') === 'menuitem') return true;
        if (element.classList.contains('dropdown-toggle')) return true;
        if (element.classList.contains('dropdown-item')) return true;

        // Ignorar elementos dentro de dropdowns
        const isInDropdown = element.closest('.dropdown-menu, [role="menu"]');
        if (isInDropdown) return true;

        return false;
    },

    setupLinkInterceptors() {
        document.addEventListener('click', (e) => {
            const target = e.target.closest('a, button');

            // Si no hay target o debe ser ignorado, salir
            if (!target || this.shouldIgnoreElement(target)) {
                return;
            }

            // Manejar descargas
            const isDownload = target.getAttribute('data-download') === 'true' ||
                target.getAttribute('download') !== null ||
                target.href?.includes('download=true');

            if (isDownload) {
                e.preventDefault();
                this.handleDownload();
                setTimeout(() => {
                    if (target.tagName === 'A') {
                        window.location.href = target.href;
                    } else {
                        target.click();
                    }
                }, 100);
            }
            // Mostrar loader solo para enlaces normales o botones submit
            else if (target.tagName === 'A' ||
                target.type === 'submit' ||
                !target.type) {
                // Verificar si el enlace tiene href y no es #
                if (target.tagName === 'A' && (!target.href || target.href.endsWith('#'))) {
                    return;
                }
                this.show();  
            }
        });
    }
};

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', () => {
    LoaderManager.init();
});