import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { Subject, takeUntil } from 'rxjs';

import { OrderService, OrderItem, OrderValidation } from '../../services/order.service';

@Component({
  selector: 'app-order-manager',
  standalone: true,
  imports: [
    CommonModule,
    DragDropModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatTooltipModule,
    MatSnackBarModule,
    MatCardModule,
    MatDividerModule
  ],
  templateUrl: './order-manager.component.html',
  styleUrls: ['./order-manager.component.scss']
})
export class OrderManagerComponent implements OnInit, OnDestroy {
  @Input() items: OrderItem[] = [];
  @Input() title: string = 'Order Management';
  @Output() orderChanged = new EventEmitter<OrderItem[]>();

  orderStatus: { status: 'valid' | 'warning' | 'error', message: string } = { status: 'valid', message: 'Order is valid' };
  private destroy$ = new Subject<void>();

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.updateOrderStatus();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onDrop(event: CdkDragDrop<OrderItem[]>): void {
    moveItemInArray(this.items, event.previousIndex, event.currentIndex);
    
    // Update display orders after drag and drop
    this.items.forEach((item, index) => {
      item.displayOrder = index + 1;
    });
    
    this.updateOrderStatus();
    this.orderChanged.emit(this.items);
  }

  moveUp(itemId: string): void {
    this.items = this.orderService.moveUp(this.items, itemId);
    this.updateOrderStatus();
    this.orderChanged.emit(this.items);
  }

  moveDown(itemId: string): void {
    this.items = this.orderService.moveDown(this.items, itemId);
    this.updateOrderStatus();
    this.orderChanged.emit(this.items);
  }

  moveToTop(itemId: string): void {
    const itemIndex = this.items.findIndex(item => item.id === itemId);
    if (itemIndex > 0) {
      const item = this.items.splice(itemIndex, 1)[0];
      this.items.unshift(item);
      this.items.forEach((item, index) => {
        item.displayOrder = index + 1;
      });
      this.updateOrderStatus();
      this.orderChanged.emit(this.items);
    }
  }

  moveToBottom(itemId: string): void {
    const itemIndex = this.items.findIndex(item => item.id === itemId);
    if (itemIndex >= 0 && itemIndex < this.items.length - 1) {
      const item = this.items.splice(itemIndex, 1)[0];
      this.items.push(item);
      this.items.forEach((item, index) => {
        item.displayOrder = index + 1;
      });
      this.updateOrderStatus();
      this.orderChanged.emit(this.items);
    }
  }

  autoReorder(): void {
    this.items = this.orderService.reorderItems(this.items);
    this.updateOrderStatus();
    this.orderChanged.emit(this.items);
  }

  validateOrder(): void {
    const validation = this.orderService.validateOrder(this.items);
    this.orderService.showValidationFeedback(validation);
  }

  showOrderInfo(): void {
    const validation = this.orderService.validateOrder(this.items);
    let message = `Total items: ${this.items.length}\n`;
    message += `Order range: ${Math.min(...this.items.map(i => i.displayOrder))} - ${Math.max(...this.items.map(i => i.displayOrder))}\n`;
    message += `Validation: ${validation.isValid ? 'Valid' : 'Invalid'}\n`;
    if (validation.errors.length > 0) {
      message += `Errors: ${validation.errors.length}\n`;
    }
    if (validation.warnings.length > 0) {
      message += `Warnings: ${validation.warnings.length}`;
    }
    
    // You can use MatSnackBar or MatDialog to show this info
    console.log('Order Info:', message);
  }

  updateOrderStatus(): void {
    this.orderStatus = this.orderService.getOrderStatus(this.items);
  }

  formatDisplayOrder(order: number): string {
    return this.orderService.formatDisplayOrder(order);
  }

  getOrderBadgeClass(order: number): string {
    // Check if this order number is duplicated
    const itemsWithSameOrder = this.items.filter(item => item.displayOrder === order);
    if (itemsWithSameOrder.length > 1) {
      return 'duplicate';
    }
    
    // Check for gaps (simplified check)
    const sortedOrders = this.items.map(i => i.displayOrder).sort((a, b) => a - b);
    const expectedOrder = sortedOrders.indexOf(order) + 1;
    if (order !== expectedOrder) {
      return 'gap';
    }
    
    return '';
  }

  getStatusIcon(status: 'valid' | 'warning' | 'error'): string {
    switch (status) {
      case 'valid': return 'check_circle';
      case 'warning': return 'warning';
      case 'error': return 'error';
      default: return 'check_circle';
    }
  }

  isFirst(itemId: string): boolean {
    return this.items.findIndex(item => item.id === itemId) === 0;
  }

  isLast(itemId: string): boolean {
    return this.items.findIndex(item => item.id === itemId) === this.items.length - 1;
  }

  isDragging(itemId: string): boolean {
    // This would be implemented with drag state tracking
    return false;
  }

  trackByItemId(index: number, item: OrderItem): string {
    return item.id;
  }
} 