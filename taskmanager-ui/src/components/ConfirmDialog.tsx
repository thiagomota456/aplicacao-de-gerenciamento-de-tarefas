import * as React from 'react';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Button from '@mui/material/Button';

type Props = {
  open: boolean;
  title?: string;
  message?: string;
  onCancel: () => void;
  onConfirm: () => void;
  confirmText?: string;
  cancelText?: string;
};

const ConfirmDialog: React.FC<Props> = ({ open, title = 'Confirmar', message, onCancel, onConfirm, confirmText = 'Confirmar', cancelText = 'Cancelar' }) => (
  <Dialog open={open} onClose={onCancel}>
    {title && <DialogTitle>{title}</DialogTitle>}
    {message && (
      <DialogContent>
        <DialogContentText>{message}</DialogContentText>
      </DialogContent>
    )}
    <DialogActions>
      <Button onClick={onCancel}>{cancelText}</Button>
      <Button onClick={onConfirm} color="error" variant="contained">{confirmText}</Button>
    </DialogActions>
  </Dialog>
);

export default ConfirmDialog;