/// <reference path="../jquery/jquery.d.ts"/>

declare var BootstrapDialog: IBootstrapDialog;

interface IBootstrapDialog {
    /** For text localization. */
    DEFAULT_TEXTS: string[];
    BUTTON_SIZES: string[];
    METHODS_TO_OVERRIDE: string[];

    TYPE_DEFAULT: string;
    TYPE_INFO: string;
    TYPE_PRIMARY: string;
    TYPE_SUCCESS: string;
    TYPE_WARNING: string;
    TYPE_DANGER: string;

    (options: IBootstrapDialogOptions): IBootstrapDialogContext;
    alert(message: string, closeCallback?: () => void): void;
    confirm(message: string, closeCallback?: (result: boolean) => void): void;
    show(options: IBootstrapDialogOptions): IBootstrapDialogInstance;
}

interface IBootstrapDialogOptions {
    /** Dialog header type. See BootstrapDialog.TYPE_xxx constants. */
    type: string;
    /** Text size. See BootstrapDialog.SIZE_xxx constants. By default - SIZE_NORMAL */
    size: string;
    /** Dialog title. Either string or JQuery element. */
    title: string|JQuery;
    /** Dialog message. Either string or JQuery element. */
    message: string|JQuery;
    /** FALSE by default. */
    closable: boolean;
    /** Whether dialog will close by clicking outside of it. */
    closeByBackdrop: boolean;
    /** Whether dialog will close by ESC. */
    closeByKeyboard: boolean;
    /** Whether fade-out background while showing the dialog. TRUE by default. */
    animate: boolean;
    /** Whether dialog could be dragged by its header. Cursor could be changed (see doc)! FALSE by default. */
    draggable: boolean;
    description: string;
    /** Default button title. OK by default. */
    buttonLabel: string;
    buttons: IBootstrapDialogButton[];
    /** Result will be true if button was click, while it will be false if users close the dialog directly. */
    callback: (result: boolean) => void;
    onshow(dialog?: IBootstrapDialogContext): void;
    onshown(dialog?: IBootstrapDialogContext): void;
    /** Return FALSE to don`t close the dialog. Don`t return anything by default. */
    onhide(dialog?: IBootstrapDialogContext): any;
    onhidden(dialog?: IBootstrapDialogContext): void;

    /** 'Cancel' by default. */
    btnCancelLabel: string;
    /** 'OK' by default. */
    btnOKLabel: string;
    /** If you didn't specify it, dialog type will be used. */
    btnOKClass: string;
} 

interface IBootstrapDialogInstance {
    $modal: JQuery;
    $modalBody: JQuery;
    $modalContent: JQuery;
    $modalDialog: JQuery;
    $modalHeader: JQuery;
    $modalFooter: JQuery;
    options: IBootstrapDialogOptions;
    opened: boolean;
}

interface IBootstrapDialogButton {
    id: string;
    label: string;
    /** Hotkey char code */
    hotkey: number;
    icon: string;
    cssClass: string;
    autospin: boolean;
    action: (dialog: IBootstrapDialogContext) => void;
}

interface IBootstrapDialogContext {
    open(): void;
    close(): void;
    realize(): void;
    setTitle(title: string): void;
    setMessage(message: string): void;
    setData(dataName: string, value: any): void;
    getData(dataName: string): any;
    getButton(buttonId: string): JQuery;
    setClosable(closable: boolean): void;
    /** See BootstrapDialog.TYPE_xxx constants. */
    setType(dialogType: string): void;
    /** Enable or disable all dialog`s buttons at once. */
    enableButtons(enable: boolean): void;
    getModalHeader(): any;
    getModalFooter(): any;
    getModalBody(): JQuery;
}