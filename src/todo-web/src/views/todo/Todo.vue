<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import { useConfirm } from 'primevue/useconfirm';
import { useAppToast } from '@/composables/useAppToast';
import { useLoadingStore } from '@/stores/useLoadingStore';
import { zodResolver } from '@primevue/forms/resolvers/zod';
import { z } from 'zod';
import { getTodos, createTodo, updateTodo, deleteTodo } from '@/services/todo-api';
import { API_CODE } from '@/constants/api-response-code';
import { formatTWDateTime } from '@/utils/format';
import FormFieldError from '@/components/FormFieldError.vue';

const confirm = useConfirm();
const toast = useAppToast();
const loadingStore = useLoadingStore();

// 列表資料
const todos = ref([]);
const selectedTodos = ref([]);

// 搜尋篩選
const searchTitle = ref('');
const filterIsComplete = ref(null);

// Dialog 狀態
const dialogVisible = ref(false);
const isEditMode = ref(false);
const currentTodoId = ref(null);

// 表單初始值（必須使用 reactive）
const initialValues = reactive({
  todoTitle: '',
  todoContent: '',
  isComplete: 'N',
});

// Zod 驗證 schema - 新增
const createSchema = z.object({
  todoTitle: z
    .string()
    .trim()
    .min(1, { error: '待辦標題為必填' })
    .max(100, { error: '待辦標題最多 100 字' }),
  todoContent: z.string().max(500, { error: '待辦內容最多 500 字' }).optional().or(z.literal('')),
});

// Zod 驗證 schema - 編輯
const editSchema = z.object({
  todoTitle: z
    .string()
    .trim()
    .min(1, { error: '待辦標題為必填' })
    .max(100, { error: '待辦標題最多 100 字' }),
  todoContent: z.string().max(500, { error: '待辦內容最多 500 字' }).optional().or(z.literal('')),
  isComplete: z.string().regex(/^[YN]$/, { error: 'IsComplete 必須為 Y 或 N' }),
});

// 動態 resolver
const resolver = computed(() => {
  return zodResolver(isEditMode.value ? editSchema : createSchema);
});

// IsComplete 選項
const isCompleteOptions = [
  { label: '未完成', value: 'N' },
  { label: '已完成', value: 'Y' },
];

// 篩選選項
const filterOptions = [
  { label: '全部', value: null },
  { label: '未完成', value: 'N' },
  { label: '已完成', value: 'Y' },
];

// 載入待辦清單
const loadTodos = async () => {
  loadingStore.show();
  try {
    const params = {};
    if (searchTitle.value.trim()) {
      params.todoTitle = searchTitle.value.trim();
    }
    if (filterIsComplete.value !== null) {
      params.isComplete = filterIsComplete.value;
    }

    const { data } = await getTodos(params);
    if (data?.code === API_CODE.SUCCESS && data?.data) {
      todos.value = data.data;
    } else {
      todos.value = [];
    }
  } catch {
    // 錯誤由 axios interceptor 統一處理
    todos.value = [];
  } finally {
    loadingStore.hide();
  }
};

// 搜尋篩選變更時重新載入
const onSearchChange = () => {
  loadTodos();
};

const onFilterChange = () => {
  loadTodos();
};

// 開啟新增 Dialog
const openCreateDialog = () => {
  isEditMode.value = false;
  currentTodoId.value = null;
  initialValues.todoTitle = '';
  initialValues.todoContent = '';
  initialValues.isComplete = 'N';
  dialogVisible.value = true;
};

// 開啟編輯 Dialog
const openEditDialog = (todo) => {
  isEditMode.value = true;
  currentTodoId.value = todo.todoId;
  initialValues.todoTitle = todo.todoTitle;
  initialValues.todoContent = todo.todoContent || '';
  initialValues.isComplete = todo.isComplete;
  dialogVisible.value = true;
};

// 關閉 Dialog
const closeDialog = () => {
  dialogVisible.value = false;
};

// 表單提交
const onFormSubmit = async ({ valid, values, reset }) => {
  if (!valid) return;

  loadingStore.show();
  try {
    if (isEditMode.value) {
      // 更新
      const { data } = await updateTodo(currentTodoId.value, {
        todoTitle: values.todoTitle,
        todoContent: values.todoContent || null,
        isComplete: values.isComplete,
      });

      if (data?.code === API_CODE.SUCCESS) {
        toast.add({
          severity: 'success',
          summary: '更新成功',
          detail: '待辦事項已更新',
        });
        closeDialog();
        reset();
        await loadTodos();
      }
    } else {
      // 新增
      const { data } = await createTodo({
        todoTitle: values.todoTitle,
        todoContent: values.todoContent || null,
      });

      if (data?.code === API_CODE.SUCCESS) {
        toast.add({
          severity: 'success',
          summary: '新增成功',
          detail: '待辦事項已新增',
        });
        closeDialog();
        reset();
        await loadTodos();
      }
    }
  } catch {
    // 錯誤由 axios interceptor 統一處理
  } finally {
    loadingStore.hide();
  }
};

// 刪除單筆
const handleDelete = (todo) => {
  confirm.require({
    message: `確定要刪除「${todo.todoTitle}」嗎？`,
    header: '確認刪除',
    icon: 'pi pi-exclamation-triangle',
    position: 'top',
    rejectProps: {
      label: '取消',
      severity: 'secondary',
      outlined: true,
    },
    acceptProps: {
      label: '刪除',
      severity: 'danger',
    },
    accept: async () => {
      loadingStore.show();
      try {
        const { data } = await deleteTodo(todo.todoId);
        if (data?.code === API_CODE.SUCCESS) {
          toast.add({
            severity: 'success',
            summary: '刪除成功',
            detail: '待辦事項已刪除',
          });
          await loadTodos();
        }
      } catch {
        // 錯誤由 axios interceptor 統一處理
      } finally {
        loadingStore.hide();
      }
    },
  });
};

// 批量刪除
const handleBatchDelete = () => {
  if (selectedTodos.value.length === 0) {
    toast.add({
      severity: 'warn',
      summary: '請選擇項目',
      detail: '請至少選擇一筆待辦事項',
    });
    return;
  }

  confirm.require({
    message: `確定要刪除選取的 ${selectedTodos.value.length} 筆待辦事項嗎？`,
    header: '確認批量刪除',
    icon: 'pi pi-exclamation-triangle',
    position: 'top',
    rejectProps: {
      label: '取消',
      severity: 'secondary',
      outlined: true,
    },
    acceptProps: {
      label: '刪除',
      severity: 'danger',
    },
    accept: async () => {
      loadingStore.show();
      try {
        const deletePromises = selectedTodos.value.map((todo) => deleteTodo(todo.todoId));
        await Promise.all(deletePromises);

        toast.add({
          severity: 'success',
          summary: '刪除成功',
          detail: `已刪除 ${selectedTodos.value.length} 筆待辦事項`,
        });
        selectedTodos.value = [];
        await loadTodos();
      } catch {
        // 錯誤由 axios interceptor 統一處理
      } finally {
        loadingStore.hide();
      }
    },
  });
};

// 初始化載入
onMounted(async () => {
  await loadTodos();
});
</script>

<template>
  <div class="card">
    <h1 class="text-3xl font-bold mb-4">Todo List</h1>

    <!-- Toolbar -->
    <Toolbar class="mb-4">
      <template #start>
        <Button label="新增" icon="pi pi-plus" severity="success" @click="openCreateDialog" />
        <Button
          label="批量刪除"
          icon="pi pi-trash"
          severity="danger"
          :disabled="selectedTodos.length === 0"
          @click="handleBatchDelete"
          class="ml-2"
        />
      </template>
      <template #end>
        <IconField iconPosition="left" class="mr-2">
          <InputIcon class="pi pi-search" />
          <InputText
            v-model="searchTitle"
            placeholder="搜尋標題..."
            @input="onSearchChange"
            style="width: 200px"
          />
        </IconField>
        <Select
          v-model="filterIsComplete"
          :options="filterOptions"
          optionLabel="label"
          optionValue="value"
          placeholder="狀態"
          @change="onFilterChange"
          style="width: 150px"
        />
      </template>
    </Toolbar>

    <!-- DataTable -->
    <DataTable
      v-model:selection="selectedTodos"
      :value="todos"
      dataKey="todoId"
      paginator
      :rows="10"
      :rowsPerPageOptions="[10, 20, 50]"
      showGridlines
      stripedRows
      tableStyle="min-width: 50rem"
    >
      <!-- 操作欄（第一欄） -->
      <Column selectionMode="multiple" headerStyle="width: 3rem" />
      <Column header="操作" headerClass="min-w-[10rem] md:min-w-[8rem]">
        <template #body="{ data }">
          <div class="flex justify-start items-center gap-4">
            <Button
              icon="pi pi-pencil"
              severity="secondary"
              @click="openEditDialog(data)"
              v-tooltip.top="'編輯'"
            />
            <Button
              icon="pi pi-trash"
              severity="danger"
              @click="handleDelete(data)"
              v-tooltip.top="'刪除'"
            />
          </div>
        </template>
      </Column>

      <!-- 其他欄位 -->
      <Column field="todoId" header="#" sortable style="width: 5rem" />
      <Column field="todoTitle" header="標題" sortable style="min-width: 12rem" />
      <Column field="todoContent" header="內容" style="min-width: 15rem">
        <template #body="{ data }">
          <span class="line-clamp-2">{{ data.todoContent || '-' }}</span>
        </template>
      </Column>
      <Column field="isComplete" header="狀態" sortable style="width: 8rem">
        <template #body="{ data }">
          <Tag
            :value="data.isComplete === 'Y' ? '已完成' : '未完成'"
            :severity="data.isComplete === 'Y' ? 'success' : 'warning'"
          />
        </template>
      </Column>
      <Column field="addTime" header="新增時間" sortable style="min-width: 10rem">
        <template #body="{ data }">
          {{ formatTWDateTime(data.addTime) }}
        </template>
      </Column>
      <Column field="addUserId" header="新增者" sortable style="min-width: 8rem" />
    </DataTable>

    <!-- 新增/編輯 Dialog -->
    <Dialog
      v-model:visible="dialogVisible"
      :header="isEditMode ? '編輯待辦事項' : '新增待辦事項'"
      :modal="true"
      :style="{ width: '30rem' }"
      @hide="closeDialog"
      position="top"
    >
      <Form
        v-slot="$form"
        :initial-values="initialValues"
        :resolver="resolver"
        @submit="onFormSubmit"
      >
        <!-- 待辦標題 -->
        <div class="flex flex-col gap-1 mb-4">
          <label
            for="todoTitle"
            class="block text-surface-900 dark:text-surface-0 font-medium mb-2"
          >
            待辦標題 <span class="text-red-500">*</span>
          </label>
          <InputText
            id="todoTitle"
            name="todoTitle"
            placeholder="請輸入待辦標題"
            fluid
            :invalid="$form.todoTitle?.invalid"
          />
          <FormFieldError :field="$form.todoTitle" />
        </div>

        <!-- 待辦內容 -->
        <div class="flex flex-col gap-1 mb-4">
          <label
            for="todoContent"
            class="block text-surface-900 dark:text-surface-0 font-medium mb-2"
          >
            待辦內容
          </label>
          <Textarea
            id="todoContent"
            name="todoContent"
            placeholder="請輸入待辦內容（選填）"
            :rows="5"
            fluid
            :invalid="$form.todoContent?.invalid"
          />
          <FormFieldError :field="$form.todoContent" />
        </div>

        <!-- 是否完成（僅編輯模式顯示） -->
        <div v-if="isEditMode" class="flex flex-col gap-1 mb-4">
          <label
            for="isComplete"
            class="block text-surface-900 dark:text-surface-0 font-medium mb-2"
          >
            是否完成 <span class="text-red-500">*</span>
          </label>
          <Select
            id="isComplete"
            name="isComplete"
            :options="isCompleteOptions"
            optionLabel="label"
            optionValue="value"
            placeholder="請選擇"
            fluid
            :invalid="$form.isComplete?.invalid"
          />
          <FormFieldError :field="$form.isComplete" />
        </div>

        <!-- 按鈕 -->
        <div class="flex justify-end gap-2 mt-4">
          <Button type="button" label="取消" severity="secondary" outlined @click="closeDialog" />
          <Button type="submit" label="儲存" />
        </div>
      </Form>
    </Dialog>
  </div>
</template>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
