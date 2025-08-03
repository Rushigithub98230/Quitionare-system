import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BehaviorSubject } from 'rxjs';

export interface OrderItem {
  id: string;
  displayOrder: number;
  name: string;
  type: 'category' | 'question' | 'questionnaire';
}

export interface OrderValidation {
  isValid: boolean;
  errors: string[];
  warnings: string[];
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private orderValidationSubject = new BehaviorSubject<OrderValidation>({
    isValid: true,
    errors: [],
    warnings: []
  });

  public orderValidation$ = this.orderValidationSubject.asObservable();

  constructor(private snackBar: MatSnackBar) {}

  /**
   * Validates the order of items and provides feedback
   */
  validateOrder(items: OrderItem[]): OrderValidation {
    const errors: string[] = [];
    const warnings: string[] = [];

    // Check for duplicate display orders
    const orderMap = new Map<number, OrderItem[]>();
    items.forEach(item => {
      if (!orderMap.has(item.displayOrder)) {
        orderMap.set(item.displayOrder, []);
      }
      orderMap.get(item.displayOrder)!.push(item);
    });

    // Find duplicates
    orderMap.forEach((itemsWithSameOrder, order) => {
      if (itemsWithSameOrder.length > 1) {
        const itemNames = itemsWithSameOrder.map(item => item.name).join(', ');
        errors.push(`Duplicate order ${order} found for: ${itemNames}`);
      }
    });

    // Check for gaps in order sequence
    const sortedItems = [...items].sort((a, b) => a.displayOrder - b.displayOrder);
    for (let i = 0; i < sortedItems.length - 1; i++) {
      const currentOrder = sortedItems[i].displayOrder;
      const nextOrder = sortedItems[i + 1].displayOrder;
      if (nextOrder - currentOrder > 1) {
        warnings.push(`Gap in order sequence: ${currentOrder} → ${nextOrder}`);
      }
    }

    // Check for negative orders
    const negativeOrders = items.filter(item => item.displayOrder < 0);
    if (negativeOrders.length > 0) {
      const itemNames = negativeOrders.map(item => item.name).join(', ');
      errors.push(`Negative order values found for: ${itemNames}`);
    }

    // Check for zero orders (might be intentional but warn)
    const zeroOrders = items.filter(item => item.displayOrder === 0);
    if (zeroOrders.length > 1) {
      warnings.push(`Multiple items with order 0 found`);
    }

    const isValid = errors.length === 0;
    const validation: OrderValidation = {
      isValid,
      errors,
      warnings
    };

    this.orderValidationSubject.next(validation);
    return validation;
  }

  /**
   * Reorders items to fix gaps and duplicates
   */
  reorderItems(items: OrderItem[]): OrderItem[] {
    const sortedItems = [...items].sort((a, b) => a.displayOrder - b.displayOrder);
    
    // Assign sequential order starting from 1
    return sortedItems.map((item, index) => ({
      ...item,
      displayOrder: index + 1
    }));
  }

  /**
   * Moves an item up in the order
   */
  moveUp(items: OrderItem[], itemId: string): OrderItem[] {
    const itemIndex = items.findIndex(item => item.id === itemId);
    if (itemIndex <= 0) return items;

    const newItems = [...items];
    const temp = newItems[itemIndex];
    newItems[itemIndex] = newItems[itemIndex - 1];
    newItems[itemIndex - 1] = temp;

    // Update display orders
    newItems.forEach((item, index) => {
      item.displayOrder = index + 1;
    });

    return newItems;
  }

  /**
   * Moves an item down in the order
   */
  moveDown(items: OrderItem[], itemId: string): OrderItem[] {
    const itemIndex = items.findIndex(item => item.id === itemId);
    if (itemIndex === -1 || itemIndex >= items.length - 1) return items;

    const newItems = [...items];
    const temp = newItems[itemIndex];
    newItems[itemIndex] = newItems[itemIndex + 1];
    newItems[itemIndex + 1] = temp;

    // Update display orders
    newItems.forEach((item, index) => {
      item.displayOrder = index + 1;
    });

    return newItems;
  }

  /**
   * Shows validation feedback to the user
   */
  showValidationFeedback(validation: OrderValidation): void {
    if (!validation.isValid) {
      const errorMessage = validation.errors.join('\n');
      this.snackBar.open(`Order validation errors:\n${errorMessage}`, 'Close', {
        duration: 5000,
        panelClass: ['error-snackbar']
      });
    }

    if (validation.warnings.length > 0) {
      const warningMessage = validation.warnings.join('\n');
      this.snackBar.open(`Order warnings:\n${warningMessage}`, 'Close', {
        duration: 3000,
        panelClass: ['warning-snackbar']
      });
    }
  }

  /**
   * Gets order status for display
   */
  getOrderStatus(items: OrderItem[]): { status: 'valid' | 'warning' | 'error', message: string } {
    const validation = this.validateOrder(items);
    
    if (!validation.isValid) {
      return {
        status: 'error',
        message: `${validation.errors.length} error(s) found`
      };
    }
    
    if (validation.warnings.length > 0) {
      return {
        status: 'warning',
        message: `${validation.warnings.length} warning(s) found`
      };
    }
    
    return {
      status: 'valid',
      message: 'Order is valid'
    };
  }

  /**
   * Formats display order for UI
   */
  formatDisplayOrder(order: number): string {
    return `#${order.toString().padStart(2, '0')}`;
  }

  /**
   * Gets order badge color based on status
   */
  getOrderBadgeColor(status: 'valid' | 'warning' | 'error'): string {
    switch (status) {
      case 'valid': return 'primary';
      case 'warning': return 'accent';
      case 'error': return 'warn';
      default: return 'primary';
    }
  }
} 