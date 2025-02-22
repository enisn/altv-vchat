import { container } from 'tsyringe';
import { CommandService } from '../services/command.service';
import type { CommandHandler } from '../types';

/**
 * Registers a command handler to the specified command.
 */
export function registerCmd(cmdName: string, handler: CommandHandler) {
    container.resolve(CommandService).register(cmdName, handler);
}

export function registerCallbackCmd(cmdName: string, callback: string) {
    container.resolve(CommandService).registerCallback(cmdName, callback);
}

/**
 * Unregisters a command handler from the specified command.
 */
export function unregisterCmd(cmdName: string) {
    return container.resolve(CommandService).unregister(cmdName);
}
