import { Component, TemplateRef, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import {
  TodoListsClient, TodoItemsClient,
  TodoListDto, TodoItemDto, PriorityLevelDto, TodoTagDto,
  CreateTodoListCommand, UpdateTodoListCommand,
  CreateTodoItemCommand, UpdateTodoItemDetailCommand,
  CreateTodoTagCommand,
  TodoTagsClient
} from '../web-api-client';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-todo-component',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})

export class TodoComponent implements OnInit {
  debug = false;
  deleting = false;
  deleteCountDown = 0;
  deleteCountDownInterval: any;
  lists: TodoListDto[];
  priorityLevels: PriorityLevelDto[];
  selectedList: TodoListDto;
  selectedItem: TodoItemDto;
  newListEditor: any = {};
  listOptionsEditor: any = {};
  newListModalRef: BsModalRef;
  listOptionsModalRef: BsModalRef;
  deleteListModalRef: BsModalRef;
  itemDetailsModalRef: BsModalRef;
  itemDetailsFormGroup = this.fb.group({
    id: [null],
    listId: [null],
    priority: [''],
    note: [''],
    tags: [[]]
  });
  supportedColours = [
    { name: 'White', code: '#FFFFFF' },
    { name: 'Red', code: '#FF5733' },
    { name: 'Orange', code: '#FFC300' },
    { name: 'Yellow', code: '#FFFF66' },
    { name: 'Green', code: '#CCFF99' },
    { name: 'Blue', code: '#6666FF' },
    { name: 'Purple', code: '#9966CC' },
    { name: 'Grey', code: '#999999' },
  ];

  // Tag properties
  tagCtrl = new FormControl();
  tags: TodoTagDto[];
  selectedFilterTag: TodoTagDto;

  constructor(
    private listsClient: TodoListsClient,
    private itemsClient: TodoItemsClient,
    private tagsClient: TodoTagsClient,
    private modalService: BsModalService,
    private fb: FormBuilder
  ) {
  }

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.listsClient.get().subscribe(
      result => {
        this.lists = result.lists;
        this.priorityLevels = result.priorityLevels;
        this.tags = result.tags;

        if (this.lists.length) {
          this.selectedList = this.lists[0];
        }
      },
      error => {
        console.error('Error loading todos:', error);
        alert('Failed to load todos.');
      }
    );
  }

  addTag() {
    const newTagName = prompt('Enter new tag name:');
    if (newTagName && newTagName.trim()) {
      // Check if tag already exists
      const exists = this.tags.some(tag =>
        tag.tag.toLowerCase() === newTagName.trim().toLowerCase()
      );

      const tag = { id: null, tag: newTagName.trim() } as TodoTagDto;

      if (!exists) {
        // Add new tag to the array
        this.tagsClient.create(tag as CreateTodoTagCommand).subscribe(
          result => {
            tag.id = result;
            this.tags.push(tag);
          },
          error => {
            const errors = JSON.parse(error.response);

            if (errors && errors.Tag) {
              this.listOptionsEditor.error = errors.Tag[0];
            }
          }
        );
      } else {
        alert('Tag already exists!');
      }
    }
  }

  deleteTag(id: number) {
    // Add new tag to the array
    this.tagsClient.delete(id).subscribe(
      result => {
        this.tags = this.tags.filter(tag => tag.id !== id);
      },
      error => {
        alert('Failed to delete tag.');
      }
    );
  }

  onSelectionTagChange(event: any) {
    const selectedTags = this.tags.filter(tag => event.value.includes(tag.id));
    this.itemDetailsFormGroup.get('tags')?.setValue(selectedTags);
  }

  // Filter items through tag
  selectTag(tag: TodoTagDto): void {
    this.selectedFilterTag = (this.selectedFilterTag && tag.tag === this.selectedFilterTag.tag)
      ? null
      : tag;
  }

  // Lists
  remainingItems(list: TodoListDto): number {
    return list.items.filter(t => !t.done).length;
  }

  showNewListModal(template: TemplateRef<any>): void {
    this.newListModalRef = this.modalService.show(template);
    this.newListEditor = {
      colour: this.supportedColours[0].code
    };

    setTimeout(() => document.getElementById('title')?.focus(), 250);
  }

  newListCancelled(): void {
    this.newListModalRef.hide();
    this.newListEditor = {};
  }

  addList(): void {
    const list = {
      id: 0,
      title: this.newListEditor.title,
      colour: this.newListEditor.colour || this.supportedColours[0].code,
      items: [],
    } as TodoListDto;

    this.listsClient.create(list as CreateTodoListCommand).subscribe(
      result => {
        list.id = result;
        this.lists.push(list);
        this.selectedList = list;
        this.newListModalRef.hide();
        this.newListEditor = {};
      },
      error => {
        const errors = JSON.parse(error.response);

        if (errors && errors.Title) {
          this.newListEditor.error = errors.Title[0];
        }

        setTimeout(() => document.getElementById('title').focus(), 250);
      }
    );
  }

  showListOptionsModal(template: TemplateRef<any>) {
    this.listOptionsEditor = {
      id: this.selectedList.id,
      title: this.selectedList.title,
      colour: this.selectedList.colour || this.supportedColours[0].code
    };

    this.listOptionsModalRef = this.modalService.show(template);
  }

  updateListOptions() {
    const list = this.listOptionsEditor as UpdateTodoListCommand;

    this.listsClient.update(this.selectedList.id, list).subscribe(
      () => {
        this.selectedList.title = this.listOptionsEditor.title;
        this.selectedList.colour = this.listOptionsEditor.colour;

        this.listOptionsModalRef.hide();
        this.listOptionsEditor = {};
      },
      error => console.error(error)
    );
  }

  confirmDeleteList(template: TemplateRef<any>) {
    this.listOptionsModalRef.hide();
    this.deleteListModalRef = this.modalService.show(template);
  }

  deleteListConfirmed(): void {
    this.listsClient.delete(this.selectedList.id).subscribe(
      () => {
        this.deleteListModalRef.hide();
        this.lists = this.lists.filter(t => t.id !== this.selectedList.id);
        this.selectedList = this.lists.length ? this.lists[0] : null;
      },
      error => console.error(error)
    );
  }

  // Items
  showItemDetailsModal(template: TemplateRef<any>, item: TodoItemDto): void {
    this.selectedItem = item;
    const selectedTagIds = item.tags?.map(tag => tag.id) || [];

    // Update form group
    this.itemDetailsFormGroup.patchValue({
      ...item,
      tags: this.tags.filter(tag => selectedTagIds.includes(tag.id)) // populate tag objects in form
    });

    this.tagCtrl.setValue([...selectedTagIds]);

    this.itemDetailsModalRef = this.modalService.show(template);
    this.itemDetailsModalRef.onHidden.subscribe(() => {
      this.stopDeleteCountDown();
    });
  }

  updateItemDetails(): void {
    const item = this.itemDetailsFormGroup.value as UpdateTodoItemDetailCommand;

    this.itemsClient.updateItemDetails(this.selectedItem.id, item).subscribe(
      () => {
        this.selectedItem.priority = item.priority;
        this.selectedItem.note = item.note;
        this.selectedItem.listId = item.listId;
        this.selectedItem.tags = item.tags;

        // If list changed, move the item
        if (this.selectedItem.listId !== item.listId) {
          this.selectedList.items = this.selectedList.items.filter(
            i => i.id !== this.selectedItem.id
          );
          const listIndex = this.lists.findIndex(l => l.id === item.listId);
          this.lists[listIndex].items.push(this.selectedItem);
        }

        this.itemDetailsModalRef.hide();
        this.itemDetailsFormGroup.reset();
      },
      error => alert(error)
    );
  }

  addItem() {
    const item = {
      id: 0,
      listId: this.selectedList.id,
      priority: this.priorityLevels[0].value,
      title: '',
      done: false
    } as TodoItemDto;

    this.selectedList.items.push(item);
    const index = this.selectedList.items.length - 1;
    this.editItem(item, 'itemTitle' + index);
  }

  editItem(item: TodoItemDto, inputId: string): void {
    this.selectedItem = item;
    setTimeout(() => document.getElementById(inputId).focus(), 100);
  }

  updateItem(item: TodoItemDto, pressedEnter: boolean = false): void {
    const isNewItem = item.id === 0;

    if (!item.title.trim()) {
      this.deleteItem(item);
      return;
    }

    if (item.id === 0) {
      this.itemsClient
        .create({
          ...item, listId: this.selectedList.id
        } as CreateTodoItemCommand)
        .subscribe(
          result => {
            item.id = result;
          },
          error => console.error(error)
        );
    } else {
      this.itemsClient.update(item.id, item).subscribe(
        () => console.log('Update succeeded.'),
        error => console.error(error)
      );
    }

    this.selectedItem = null;

    if (isNewItem && pressedEnter) {
      setTimeout(() => this.addItem(), 250);
    }
  }

  deleteItem(item: TodoItemDto, countDown?: boolean) {
    if (countDown) {
      if (this.deleting) {
        this.stopDeleteCountDown();
        return;
      }
      this.deleteCountDown = 3;
      this.deleting = true;
      this.deleteCountDownInterval = setInterval(() => {
        if (this.deleting && --this.deleteCountDown <= 0) {
          this.deleteItem(item, false);
        }
      }, 1000);
      return;
    }
    this.deleting = false;
    if (this.itemDetailsModalRef) {
      this.itemDetailsModalRef.hide();
    }

    if (item.id === 0) {
      const itemIndex = this.selectedList.items.indexOf(this.selectedItem);
      this.selectedList.items.splice(itemIndex, 1);
    } else {
      this.itemsClient.delete(item.id).subscribe(
        () =>
        (this.selectedList.items = this.selectedList.items.filter(
          t => t.id !== item.id
        )),
        error => console.error(error)
      );
    }
  }

  stopDeleteCountDown(): void {
    clearInterval(this.deleteCountDownInterval);
    this.deleteCountDown = 0;
    this.deleting = false;
  }

  isLightBackground(colour: string): boolean {
    // White and Yellow
    const lightColours = [this.supportedColours[0].code, this.supportedColours[3].code];
    return lightColours.includes(colour?.toUpperCase());
  }
}
