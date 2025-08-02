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

import { OrderService, OrderItem, OrderValidation } from '../../../core/services/order.service';

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
  template: `
    <mat-card class="order-manager-card">
      <mat-card-header>
        <mat-card-title>
          <mat-icon>sort</mat-icon>
          Order Management
        </mat-card-title>
        <mat-card-subtitle>
          Drag and drop to reorder items
        </mat-card-subtitle>
      </mat-card-header>

      <mat-card-content>
        <!-- Order Status Indicator -->
        <div class="order-status">
          <mat-chip [color]="orderStatus.status" selected>
            <mat-icon>{{ getStatusIcon(orderStatus.status) }}</mat-icon>
            {{ orderStatus.message }}
          </mat-chip>
        </div>

        <!-- Order Actions -->
        <div class="order-actions">
          <button mat-button color="primary" (click)="autoReorder()" 
                  [disabled]="items.length === 0">
            <mat-icon>auto_fix_high</mat-icon>
            Auto Reorder
          </button>
          <button mat-button color="accent" (click)="validateOrder()" 
                  [disabled]="items.length === 0">
            <mat-icon>check_circle</mat-icon>
            Validate Order
          </button>
          <button mat-button color="warn" (click)="showOrderInfo()" 
                  [disabled]="items.length === 0">
            <mat-icon>info</mat-icon>
            Order Info
          </button>
        </div>

        <!-- Drag and Drop List -->
        <div class="drag-list" cdkDropList (cdkDropListDropped)="onDrop($event)">
          <div class="drag-item" 
               *ngFor="let item of items; trackBy: trackByItemId" 
               cdkDrag
               [class.dragging]="isDragging(item.id)">
            <div class="drag-handle" cdkDragHandle>
              <mat-icon>drag_indicator</mat-icon>
            </div>
            
            <div class="item-content">
              <div class="item-header">
                <span class="order-badge" [class]="getOrderBadgeClass(item.displayOrder)">
                  {{ formatDisplayOrder(item.displayOrder) }}
                </span>
                <span class="item-name">{{ item.name }}</span>
                <span class="item-type">{{ item.type }}</span>
              </div>
              
              <div class="item-actions">
                <button mat-icon-button (click)="moveUp(item.id)" 
                        [disabled]="isFirst(item.id)"
                        matTooltip="Move Up">
                  <mat-icon>keyboard_arrow_up</mat-icon>
                </button>
                <button mat-icon-button (click)="moveDown(item.id)" 
                        [disabled]="isLast(item.id)"
                        matTooltip="Move Down">
                  <mat-icon>keyboard_arrow_down</mat-icon>
                </button>
                <button mat-icon-button (click)="moveToTop(item.id)" 
                        [disabled]="isFirst(item.id)"
                        matTooltip="Move to Top">
                  <mat-icon>vertical_align_top</mat-icon>
                </button>
                <button mat-icon-button (click)="moveToBottom(item.id)" 
                        [disabled]="isLast(item.id)"
                        matTooltip="Move to Bottom">
                  <mat-icon>vertical_align_bottom</mat-icon>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div *ngIf="items.length === 0" class="empty-state">
          <mat-icon>sort</mat-icon>
          <p>No items to order</p>
        </div>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .order-manager-card {
      margin: 16px 0;
    }

    .order-status {
      margin-bottom: 16px;
    }

    .order-actions {
      display: flex;
      gap: 8px;
      margin-bottom: 16px;
      flex-wrap: wrap;
    }

    .drag-list {
      border: 1px solid #e0e0e0;
      border-radius: 4px;
      min-height: 100px;
    }

    .drag-item {
      display: flex;
      align-items: center;
      padding: 12px;
      border-bottom: 1px solid #f0f0f0;
      background: white;
      cursor: move;
      transition: all 0.2s ease;
    }

    .drag-item:hover {
      background: #f5f5f5;
    }

    .drag-item.dragging {
      opacity: 0.5;
      background: #e3f2fd;
    }

    .drag-handle {
      margin-right: 12px;
      color: #666;
      cursor: grab;
    }

    .drag-handle:active {
      cursor: grabbing;
    }

    .item-content {
      display: flex;
      align-items: center;
      justify-content: space-between;
      flex: 1;
    }

    .item-header {
      display: flex;
      align-items: center;
      gap: 12px;
      flex: 1;
    }

    .order-badge {
      background: #3f51b5;
      color: white;
      padding: 4px 8px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: bold;
      min-width: 40px;
      text-align: center;
    }

    .order-badge.duplicate {
      background: #f44336;
    }

    .order-badge.gap {
      background: #ff9800;
    }

    .item-name {
      font-weight: 500;
      flex: 1;
    }

    .item-type {
      background: #e0e0e0;
      color: #666;
      padding: 2px 8px;
      border-radius: 12px;
      font-size: 11px;
      text-transform: uppercase;
    }

    .item-actions {
      display: flex;
      gap: 4px;
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 32px;
      color: #666;
      text-align: center;
    }

    .empty-state mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      margin-bottom: 16px;
      color: #ccc;
    }

    .cdk-drag-preview {
      box-sizing: border-box;
      border-radius: 4px;
      box-shadow: 0 5px 5px -3px rgba(0, 0, 0, 0.2),
                  0 8px 10px 1px rgba(0, 0, 0, 0.14),
                  0 3px 14px 2px rgba(0, 0, 0, 0.12);
    }

    .cdk-drag-placeholder {
      opacity: 0;
    }

    .cdk-drag-animating {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }

    .drag-list.cdk-drop-list-dragging .drag-item:not(.cdk-drag-placeholder) {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }
  `]
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