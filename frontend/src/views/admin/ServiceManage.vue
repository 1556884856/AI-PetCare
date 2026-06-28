<!--
  服务管理页面（管理员）
  列表展示 + 弹窗新增/编辑/删除服务
-->
<template>
  <div><h1 class="admin-title">🛁 服务管理</h1>
    <div class="toolbar"><el-button type="primary" @click="showDialog = true; resetForm()">+ 新增服务</el-button></div>
    <el-table :data="list" v-loading="loading" style="width: 100%;">
      <el-table-column prop="name" label="名称" />
      <el-table-column label="类别" width="100"><template #default="{ row }">{{ categoryLabel(row.category) }}</template></el-table-column>
      <el-table-column label="适用" width="80"><template #default="{ row }">{{ row.petType === 'All' ? '全部' : row.petType === 'Dog' ? '狗' : '猫' }}</template></el-table-column>
      <el-table-column label="价格" width="100"><template #default="{ row }">¥{{ row.price }}</template></el-table-column>
      <el-table-column prop="durationMinutes" label="时长(分)" width="100" />
      <el-table-column prop="sortOrder" label="排序" width="80" />
      <el-table-column label="状态" width="80"><template #default="{ row }"><el-tag :type="row.isActive ? 'success' : 'info'">{{ row.isActive ? '启用' : '禁用' }}</el-tag></template></el-table-column>
      <el-table-column label="操作" width="160">
        <template #default="{ row }"><el-button size="small" @click="editService(row)">编辑</el-button><el-button size="small" type="danger" @click="remove(row.id)">删除</el-button></template>
      </el-table-column>
    </el-table>
  </div>
  <el-dialog v-model="showDialog" :title="editingId ? '编辑服务' : '新增服务'" width="480px">
    <el-form :model="form" label-position="top">
      <el-form-item label="名称"><el-input v-model="form.name" /></el-form-item>
      <el-form-item label="描述"><el-input v-model="form.description" type="textarea" /></el-form-item>
      <el-form-item label="类别"><el-select v-model="form.category"><el-option value="Bath" label="洗浴" /><el-option value="Grooming" label="美容" /><el-option value="Spa" label="SPA" /><el-option value="Basic" label="基础护理" /></el-select></el-form-item>
      <el-form-item label="宠物类型"><el-radio-group v-model="form.petType"><el-radio value="All">全部</el-radio><el-radio value="Dog">狗</el-radio><el-radio value="Cat">猫</el-radio></el-radio-group></el-form-item>
      <el-form-item label="价格"><el-input-number v-model="form.price" :min="0" :step="1" /></el-form-item>
      <el-form-item label="时长(分钟)"><el-input-number v-model="form.durationMinutes" :min="1" /></el-form-item>
      <el-form-item label="排序"><el-input-number v-model="form.sortOrder" /></el-form-item>
      <el-form-item label="状态"><el-switch v-model="form.isActive" active-text="启用" inactive-text="禁用" /></el-form-item>
    </el-form>
    <template #footer><el-button @click="showDialog = false">取消</el-button><el-button type="primary" @click="save">保存</el-button></template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getServices, createService, updateService, deleteService } from '@/api/services'
import { ElMessage, ElMessageBox } from 'element-plus'
const list = ref<any[]>([]); const loading = ref(false); const showDialog = ref(false); const editingId = ref<number | null>(null)
const form = reactive({ name: '', description: '', category: 'Bath', petType: 'All', price: 0, durationMinutes: 60, sortOrder: 99, isActive: true })
const resetForm = () => { editingId.value = null; Object.assign(form, { name: '', description: '', category: 'Bath', petType: 'All', price: 0, durationMinutes: 60, sortOrder: 99, isActive: true }) }
const categoryLabel = (c: string) => ({ Bath: '洗浴', Grooming: '美容', Spa: 'SPA', Basic: '基础护理' } as any)[c] || c
const editService = (s: any) => { editingId.value = s.id; Object.assign(form, s); showDialog.value = true }
const fetchList = async () => { loading.value = true; try { const res: any = await getServices(); list.value = res.data || [] } finally { loading.value = false } }
onMounted(fetchList)
const save = async () => { try { if (editingId.value) await updateService(editingId.value, form); else await createService(form); ElMessage.success('保存成功'); showDialog.value = false; fetchList() } catch {} }
const remove = async (id: number) => { await ElMessageBox.confirm('确定删除该服务吗？', '提示', { type: 'warning' }); try { await deleteService(id); ElMessage.success('已删除'); fetchList() } catch {} }
</script>
<style scoped>
.admin-title { font-size: 24px; margin-bottom: 24px; color: #333; }
.toolbar { margin-bottom: 16px; }
</style>
