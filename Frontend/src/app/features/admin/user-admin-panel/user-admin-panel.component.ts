import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../core/services/admin.service';
import { ToastService } from '../../../core/services/toast.service';
import { BehaviorSubject } from 'rxjs';
import { AsyncPipe, CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GuestService } from '../../../core/services/guest.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-admin-panel',
  imports: [AsyncPipe, DatePipe, CommonModule, FormsModule, RouterLink],
  templateUrl: './user-admin-panel.component.html',
  styleUrl: './user-admin-panel.component.css',
})
export class UserAdminPanelComponent implements OnInit {
  users$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);
  artisans: string[] = [];
  categories: string[] = [];

  constructor(
    private adminService: AdminService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.applyFilters();
  }

  standardKeys = [
    'id',
    'username',
    'email',
    'status',
    'role',
    'isDeleted',
    'createdAt',
    'updatedAt',
    'deletedAt',
  ];

  getExtraKeys(user: any): string[] {
    return Object.keys(user).filter((key) => !this.standardKeys.includes(key));
  }

  toggleActive(user: any) {
    this.adminService.setUserStatus(user.id).subscribe({
      next: () => {
        // Update the user's status in the local array
        user.status = user.status === 'Active' ? 'Inactive' : 'Active';
        this.toastService.show({
          text: `User status updated to ${user.status}`,
          classname: 'bg-success text-light',
          delay: 3000,
        });
      },
      error: (error) => {
        console.error('Error updating user status:', error);
        this.toastService.show({
          text: `Error updating user status: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }

  toggleDelete(user: any) {
    const newValue = !user.isDeleted;
    this.adminService.setUserDeleted(user.id).subscribe({
      next: () => {
        user.isDeleted = newValue;
        if (user.isDeleted) {
          user.status = 'Inactive';
        }
        this.toastService.show({
          text: `User ${newValue ? 'deleted' : 'restored'} successfully`,
          classname: 'bg-success text-light',
          delay: 3000,
        });
      },
      error: (error) => {
        console.error('Error updating user deletion status:', error);
        this.toastService.show({
          text: `Error updating user deletion status: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }

  filters: any = {
    role: '',
    status: '',
    search: '',
  };

  applyFilters() {
    this.adminService.getAllUsers(this.filters).subscribe((users) => {
      this.users$.next(users);
    });
  }
}
