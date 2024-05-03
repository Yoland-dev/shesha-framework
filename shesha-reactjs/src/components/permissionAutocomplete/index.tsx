import React, { FC, useState } from 'react';
import { Select } from 'antd';
import { PermissionsTree } from '@/components/permissionsTree';

export interface IPropertyAutocompleteProps {
  onChange?: (value: string[]) => void;
  value?: string[];

}

const PermissionAutocomplete: FC<IPropertyAutocompleteProps> = (props) => {

  const { onChange, value } = props;

  const [searchText, setSearchText] = useState('');

  const internalOnChange = (values?: string[]) => {
    //setSearchText('');
    if (onChange) {
      onChange(values);
    }
  };

  return (
    <Select
      style={{width: '100%'}}
      mode="multiple"
      allowClear
      onSearch={setSearchText}
      searchValue={searchText}

      dropdownStyle={{ maxHeight: '50%', overflow: 'auto' }}
      dropdownRender={_ => (

        <PermissionsTree 
          formComponentId={''} 
          formComponentName={''} 
          mode={'Select'}
          hideSearch

          searchText={searchText}
          onChange={internalOnChange}
          value={value}
        />
      )}
      onChange={internalOnChange}
      value={value}
    />
  );
};

export default PermissionAutocomplete;
